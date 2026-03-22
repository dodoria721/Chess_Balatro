using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TacticManager : MonoBehaviour
{
    public static TacticManager Instance;

    [Header("Owned Tactic")]
    [SerializeField] private List<TacticScriptableObject> allTactics = new List<TacticScriptableObject>();

    void Awake() => Instance = this;

    public (float scoreSum, float multSum) GetCurrentTacticBonus(List<PieceController> movedPieces)
    {
        float totalScore = 0f;
        float totalMult = 0f;

        var whitePieces = BoardManager.Instance.piecePositions.Values.Where(p => p.MyTeam == Team.White).ToList();
        var blackPieces = BoardManager.Instance.piecePositions.Values.Where(p => p.MyTeam == Team.Black).ToList();

        // 1. 공격 맵 생성 (어떤 흑색 기물이 어떤 백색 기물들에게 공격받는가)
        Dictionary<PieceController, List<PieceController>> attackMap = new Dictionary<PieceController, List<PieceController>>();
        foreach (var black in blackPieces)
        {
            attackMap[black] = whitePieces.Where(w => w.GetAttackRange().Contains(black.CurrentPos)).ToList();
        }

        // 2. 방어 맵 및 과부하 판정용 역방향 맵 생성
        // defenseMap: target(피방어자) -> 누가 지켜주는가
        // defenderMap: defender(방어자) -> 누구누구를 지키고 있는가 (과부하 체크용)
        Dictionary<PieceController, List<PieceController>> defenseMap = new Dictionary<PieceController, List<PieceController>>();
        Dictionary<PieceController, List<PieceController>> defenderMap = new Dictionary<PieceController, List<PieceController>>();

        foreach (var target in blackPieces)
        {
            var protectors = blackPieces.Where(p => p != target && p.GetAttackRange().Contains(target.CurrentPos)).ToList();
            defenseMap[target] = protectors;

            foreach (var defender in protectors)
            {
                if (!defenderMap.ContainsKey(defender)) defenderMap[defender] = new List<PieceController>();
                defenderMap[defender].Add(target);
            }
        }

        // --- [A] 타켓 중심 판정 ---
        foreach (var entry in attackMap)
        {
            PieceController targetBlack = entry.Key;
            List<PieceController> attackers = entry.Value;
            int count = attackers.Count;
            bool isBoss = IsBoss(targetBlack);

            if (count >= 1) AddBonus("Attack", ref totalScore, ref totalMult);

            if (count == 2)
            {
                if (isBoss) AddBonus("Double Check", ref totalScore, ref totalMult);
                else AddBonus("Double Attack", ref totalScore, ref totalMult);
            }

            // [기존 Overload 로직 대체] 아래 [D] 섹션에서 더 정확하게 처리합니다.

            if (count == 3) AddBonus("The Trinity", ref totalScore, ref totalMult);
            if (count >= 6) AddBonus("Full Siege", ref totalScore, ref totalMult);

            if (attackers.Count(p => p.pieceData.PieceName.Contains("Pawn")) >= 5)
                AddBonus("Peasant Revolt", ref totalScore, ref totalMult);

            if (attackers.Count(p => p.pieceData.PieceName.Contains("Knight")) >= 4)
                AddBonus("Apocalypse", ref totalScore, ref totalMult);
        }

        // --- [B] 아군 중심 판정 (포크) ---
        foreach (var white in whitePieces)
        {
            if (blackPieces.Count(b => white.GetAttackRange().Contains(b.CurrentPos)) >= 2)
                AddBonus("Fork", ref totalScore, ref totalMult);
        }

        // --- [C] 라인 스캔 판정 (디스커버드) ---
        CheckDiscoveredTactics(movedPieces, ref totalScore, ref totalMult);

        // --- [D] 과부하(Overload) 정밀 판정 ---
        foreach (var entry in defenderMap)
        {
            PieceController defender = entry.Key;
            List<PieceController> protectedPieces = entry.Value;

            // 방어자가 2개 이상의 기물을 보호하고 있고, 
            // 그 보호받는 기물 중 하나라도 현재 공격받고 있다면 '과부하' 상태로 간주
            if (protectedPieces.Count >= 2)
            {
                if (protectedPieces.Any(p => attackMap.ContainsKey(p) && attackMap[p].Count > 0))
                {
                    AddBonus("Overload", ref totalScore, ref totalMult);
                }
            }
        }

        return (totalScore, totalMult);
    }

    private void CheckDiscoveredTactics(List<PieceController> movedPieces, ref float s, ref float m)
    {
        Vector2Int[] scanDirections = {
            new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        foreach (var w in movedPieces)
        {
            Vector2Int gapPos = w.PreviousPos;

            foreach (var dir in scanDirections)
            {
                PieceController backSniper = null;
                for (int j = 1; j < 8; j++)
                {
                    Vector2Int backPos = gapPos + (-dir * j);
                    if (!IsWithinBounds(backPos)) break;

                    PieceController p = BoardManager.Instance.GetPieceAt(backPos);
                    if (p != null)
                    {
                        if (p != w && p.MyTeam == Team.White && p.pieceData.IsInfinite)
                        {
                            if (p.pieceData.MoveDirections.Any(d => d == dir || d == -dir))
                                backSniper = p;
                        }
                        break;
                    }
                }

                if (backSniper != null)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        Vector2Int frontPos = gapPos + (dir * i);
                        if (!IsWithinBounds(frontPos)) break;

                        PieceController target = BoardManager.Instance.GetPieceAt(frontPos);
                        if (target != null)
                        {
                            if (target == w) break;

                            if (target.MyTeam == Team.Black)
                            {
                                if (IsBoss(target)) AddBonus("Discovered Check", ref s, ref m);
                                else AddBonus("Discovered Attack", ref s, ref m);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    private bool IsBoss(PieceController piece)
    {
        return piece.pieceData.PieceName.Contains("King") || piece.pieceData.PieceName.Contains("Boss");
    }

    private bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < BoardManager.Instance.width &&
               pos.y >= 0 && pos.y < BoardManager.Instance.height;
    }

    private void AddBonus(string name, ref float score, ref float mult)
    {
        var data = allTactics.FirstOrDefault(t => t.TacticName == name);
        if (data != null)
        {
            score += data.BaseScore;
            mult += data.MultValue;
            Debug.Log($"<color=#00FF00>[전술]</color> {name} 발동! (+{data.BaseScore} / x{data.MultValue})");
        }
    }
}