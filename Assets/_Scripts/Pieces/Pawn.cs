using UnityEngine;

public class Pawn : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        Vector2Int diff = targetPos - currentGridPos;

        // 데이터(MoveDirections)에서 첫 번째 값을 전진 방향으로 사용
        if (currentMoveDirections == null || currentMoveDirections.Length == 0) return false;
        Vector2Int forwardDir = currentMoveDirections[0];

        PieceController targetPiece = BoardManager.Instance.GetPieceAt(targetPos);

        // 1. 직선 전진 로직
        if (targetPiece == null)
        {
            if (diff == forwardDir) return true;
            if (isFirstMove && diff == forwardDir * 2)
            {
                if (BoardManager.Instance.GetPieceAt(currentGridPos + forwardDir) == null) return true;
            }
        }
        else if (targetPiece != null)
        {
            if (targetPiece.MyTeam == this.MyTeam) return false;
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