using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAIManager : MonoBehaviour
{
    public IEnumerator PlayTurn()
    {
        Debug.Log("적의 턴 시작 (순서: 우측 하단 -> 좌측 상단)");

        // 1. 적(Black) 기물 수집 및 정렬
        // Y 기준 오름차순(OrderBy): 아래 줄(0, 1, 2...)부터 위로
        // X 기준 내림차순(ThenByDescending): 같은 줄에선 오른쪽(7, 6, 5...)부터 왼쪽으로
        var myPieces = BoardManager.Instance.piecePositions.Values
            .Where(p => p != null && p.MyTeam == Team.Black)
            .OrderBy(p => p.GetCurrentGridPos().y)
            .ThenByDescending(p => p.GetCurrentGridPos().x)
            .ToList();

        // 2. 정렬된 리스트 순서대로 행동 수행
        foreach (var piece in myPieces)
        {
            if (piece == null) continue;

            Vector2Int bestMove = DecideBestMove(piece);

            if (bestMove != new Vector2Int(-1, -1))
            {
                // 기물 선택 연출을 위한 짧은 대기
                yield return new WaitForSeconds(0.2f);

                // 공격 대상(White) 확인 및 제거
                PieceController target = BoardManager.Instance.GetPieceAt(bestMove);
                if (target != null && target.MyTeam == Team.White)
                {
                    Destroy(target.gameObject);
                }

                // 월드 좌표로 이동 및 데이터 갱신
                piece.transform.position = BoardManager.Instance.GetTileWorldPos(bestMove);
                piece.OnMoveConfirmed(bestMove);

                Debug.Log($"행동 기물 위치: ({piece.GetCurrentGridPos().x}, {piece.GetCurrentGridPos().y})");

                // 기물 간 이동 간격 (하나씩 움직이는 연출)
                yield return new WaitForSeconds(0.8f);
            }
        }

        Debug.Log("적의 모든 기물 이동 종료.");
    }

    private Vector2Int DecideBestMove(PieceController piece)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        for (int x = 0; x < BoardManager.Instance.width; x++)
        {
            for (int y = 0; y < BoardManager.Instance.height; y++)
            {
                Vector2Int targetPos = new Vector2Int(x, y);
                if (piece.IsValidMove(targetPos)) possibleMoves.Add(targetPos);
            }
        }

        if (possibleMoves.Count == 0) return new Vector2Int(-1, -1);

        // 공격 우선 전략
        foreach (var move in possibleMoves)
        {
            var t = BoardManager.Instance.GetPieceAt(move);
            if (t != null && t.MyTeam == Team.White) return move;
        }

        // 공격할 곳이 없으면 랜덤 이동
        return possibleMoves[Random.Range(0, possibleMoves.Count)];
    }
}