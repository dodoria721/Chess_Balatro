using UnityEngine;

public class Pawn : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        // 제자리/보드밖/아군체크 등 아주 기초적인 것만 부모에게 맡김
        if (targetPos == currentGridPos) return false;

        Vector2Int diff = targetPos - currentGridPos;
        Vector2Int forwardDir = currentMoveDirections[0]; // 보통 (0, 1) 또는 (0, -1)
        PieceController targetPiece = BoardManager.Instance.GetPieceAt(targetPos);

        // 1. 직선 전진 (도착지에 아무도 없어야 함)
        if (targetPiece == null)
        {
            // 한 칸 전진
            if (diff == forwardDir) return true;
            // 첫 이동 두 칸 전진
            if (isFirstMove && diff == forwardDir * 2)
            {
                // 중간 경로가 비어있어야 함
                if (BoardManager.Instance.GetPieceAt(currentGridPos + forwardDir) == null) return true;
            }
        }
        // 2. 대각선 공격 (도착지에 적이 있어야 함)
        else if (targetPiece.MyTeam != this.MyTeam)
        {
            if (IsDiagonalAttack(diff, forwardDir)) return true;
        }

        return false;
    }

    // 공격 방향을 판별하는 보조 함수
    private bool IsDiagonalAttack(Vector2Int diff, Vector2Int forward)
    {
        // 수학적 트릭: 전진 벡터와 diff의 내적이나 좌표 변환을 쓸 수 있지만,
        // 가장 직관적인 방법은 '전진 방향'과 '옆 방향'을 조합하는 것입니다.

        // 전진 방향에 수직인 벡터(옆방향)를 구함
        Vector2Int side = new Vector2Int(-forward.y, forward.x);

        // 왼쪽 대각선 = 전진 + 옆
        // 오른쪽 대각선 = 전진 - 옆
        return diff == (forward + side) || diff == (forward - side);
    }
}