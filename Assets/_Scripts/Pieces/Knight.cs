using UnityEngine;

public class Knight : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        Vector2Int diff = targetPos - currentGridPos;

        PieceController targetPiece = BoardManager.Instance.GetPieceAt(targetPos);

        if (targetPiece != null)
        {
            if (targetPiece.MyTeam == this.MyTeam) return false;
        }

        if (currentMoveDirections != null)
        {
            foreach (Vector2Int dir in currentMoveDirections)
            {
                if (diff == dir) return true;
            }
        }


        return false;
    }
}