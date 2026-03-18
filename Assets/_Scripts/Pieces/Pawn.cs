using UnityEngine;

public class Pawn : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        Vector2Int diff = targetPos - currentGridPos;
        int forward = 1; // 화이트 전진 방향

        PieceController targetPiece = BoardManager.Instance.GetPieceAt(targetPos);

        // 1. 직선 전진: 앞에 기물이 없어야 함
        if (diff.x == 0 && targetPiece == null)
        {
            if (diff.y == forward) return true;
            if (isFirstMove && diff.y == forward * 2)
            {
                if (BoardManager.Instance.GetPieceAt(currentGridPos + new Vector2Int(0, forward)) == null) return true;
            }
        }

        // 2. 대각선 공격: 목적지에 적 기물이 있어야 함
        if (Mathf.Abs(diff.x) == 1 && diff.y == forward && targetPiece != null)
        {
            return true;
        }

        return false;
    }
}