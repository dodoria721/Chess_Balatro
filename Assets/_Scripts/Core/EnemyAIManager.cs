using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAIManager : MonoBehaviour
{
    public IEnumerator PlayTurn()
    {
        // 1. 적(Black) 기물 수집 및 정렬
        // GetCurrentGridPos() 대신 아까 추가한 CurrentPos 프로퍼티를 사용합니다.
        var myPieces = BoardManager.Instance.piecePositions.Values
            .Where(p => p != null && p.MyTeam == Team.Black)
            .OrderBy(p => p.CurrentPos.y)  // 수정됨
            .ThenByDescending(p => p.CurrentPos.x) // 수정됨
            .ToList();

        // 2. 정렬된 리스트 순서대로 행동 수행
        foreach (var piece in myPieces)
        {
            if (piece == null) continue;

            Vector2Int bestMove = DecideBestMove(piece);

            // 이동 가능한 위치가 (-1, -1)이 아닐 때만 실행
            if (bestMove != new Vector2Int(-1, -1))
            {
                yield return new WaitForSeconds(0.2f);

                PieceController target = BoardManager.Instance.GetPieceAt(bestMove);
                if (target != null && target.MyTeam == Team.White)
                {
                    // [중요] 딕셔너리에서 먼저 제거하여 유령 기물이 남지 않게 함
                    BoardManager.Instance.piecePositions.Remove(bestMove);
                    Destroy(target.gameObject);
                }

                // 기물 위치 업데이트 및 보드 등록
                piece.transform.position = BoardManager.Instance.GetTileWorldPos(bestMove);
                piece.OnMoveConfirmed(bestMove); // 내부에서 BoardManager 업데이트 및 currentGridPos 갱신 수행

                yield return new WaitForSeconds(0.8f);
            }
        }

        Debug.Log("적의 모든 기물 이동 종료.");
    }

    private Vector2Int DecideBestMove(PieceController piece)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 보드 전체를 순회하며 이동 가능한 좌표 수집
        for (int x = 0; x < BoardManager.Instance.width; x++)
        {
            for (int y = 0; y < BoardManager.Instance.height; y++)
            {
                Vector2Int targetPos = new Vector2Int(x, y);
                // IsValidMove를 사용하여 이동 규칙에 맞는지 확인
                if (piece.IsValidMove(targetPos))
                {
                    possibleMoves.Add(targetPos);
                }
            }
        }

        if (possibleMoves.Count == 0) return new Vector2Int(-1, -1);

        // 1순위 전략: 공격 (아군 기물인 White가 있는 곳으로 이동)
        foreach (var move in possibleMoves)
        {
            var t = BoardManager.Instance.GetPieceAt(move);
            if (t != null && t.MyTeam == Team.White) return move;
        }

        // 2순위 전략: 랜덤 이동 (공격할 대상이 없을 때)
        return possibleMoves[Random.Range(0, possibleMoves.Count)];
    }
}