using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public int searchDepth = 3;
    public Team aiTeam = Team.Black;

    public void ExecuteTurn()
    {
        StartCoroutine(AIProcessRoutine());
    }

    IEnumerator AIProcessRoutine()
    {
        BoardManager.Instance.currentState = GameState.Busy;

        Debug.Log("<color=yellow>🤖 AI가 최선의 수를 시뮬레이션 중입니다...</color>");

        // 탐색 시작 시간 측정 (성능 체크용)
        float startTime = Time.realtimeSinceStartup;

        var result = Minimax(searchDepth, float.MinValue, float.MaxValue, true);

        float duration = Time.realtimeSinceStartup - startTime;

        yield return new WaitForSeconds(1.0f);

        if (result.bestMove.piece != null)
        {
            // [디버그] 최종 결정된 수 출력
            Debug.Log($"<color=cyan>✅ AI 결정: {result.bestMove.piece.name}을(를) " +
                      $"{result.bestMove.fromPos}에서 {result.bestMove.toPos}로 이동 " +
                      $"(예측 점수: {result.evaluation}, 소요 시간: {duration:F2}s)</color>");

            BoardManager.Instance.MovePiece(result.bestMove.fromPos, result.bestMove.toPos, result.bestMove.piece);
        }
        else
        {
            Debug.LogWarning("⚠️ AI가 가능한 수를 찾지 못했습니다.");
        }

        BoardManager.Instance.currentState = GameState.PlayerTurn;
    }

    private (Move bestMove, float evaluation) Minimax(int depth, float alpha, float beta, bool maximizing)
    {
        if (depth == 0) return (default, EvaluateBoard());

        Team currentTeam = maximizing ? aiTeam : (aiTeam == Team.White ? Team.Black : Team.White);
        List<Move> allMoves = GetAllMoves(currentTeam);

        if (allMoves.Count == 0) return (default, EvaluateBoard());

        Move bestMove = allMoves[0];

        if (maximizing)
        {
            float maxEval = float.MinValue;
            foreach (var move in allMoves)
            {
                // [디버그] 가장 상위 뎁스에서 어떤 기물을 시뮬레이션 중인지 출력
                if (depth == searchDepth)
                {
                    Debug.Log($"🔍 시뮬레이션 중: {move.piece.name} -> {move.toPos}");
                }

                BoardManager.Instance.SimulateMove(move);
                float eval = Minimax(depth - 1, alpha, beta, false).evaluation;
                BoardManager.Instance.UndoMove(move);

                if (eval > maxEval) { maxEval = eval; bestMove = move; }
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return (bestMove, maxEval);
        }
        else
        {
            float minEval = float.MaxValue;
            foreach (var move in allMoves)
            {
                BoardManager.Instance.SimulateMove(move);
                float eval = Minimax(depth - 1, alpha, beta, true).evaluation;
                BoardManager.Instance.UndoMove(move);

                if (eval < minEval) { minEval = eval; bestMove = move; }
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return (bestMove, minEval);
        }
    }

    private float EvaluateBoard()
    {
        float score = 0;
        foreach (var p in BoardManager.Instance.piecePositions.Values)
        {
            // AI 팀이면 플러스, 플레이어 팀이면 마이너스
            score += (p.MyTeam == aiTeam) ? p.pieceData.PieceScore : -p.pieceData.PieceScore;
        }
        return score;
    }

    private List<Move> GetAllMoves(Team team)
    {
        List<Move> moves = new List<Move>();
        foreach (var kvp in BoardManager.Instance.piecePositions)
        {
            if (kvp.Value.MyTeam == team)
            {
                PieceController p = kvp.Value;
                for (int x = 0; x < BoardManager.Instance.width; x++)
                {
                    for (int y = 0; y < BoardManager.Instance.height; y++)
                    {
                        Vector2Int target = new Vector2Int(x, y);
                        if (p.IsValidMove(target))
                        {
                            moves.Add(new Move(p, p.GetCurrentGridPos(), target, BoardManager.Instance.GetPieceAt(target), p.isFirstMove));
                        }
                    }
                }
            }
        }
        return moves;
    }
}