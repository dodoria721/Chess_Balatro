using UnityEngine;

public class King : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        // 1. 부모가 거리(1칸)와 방향을 이미 체크함
        if (!base.IsValidMove(targetPos)) return false;

        // 2. 도착지 기물 체크만 하면 끝 (한 칸 이동이라 중간 경로 체크 불필요)
        PieceController targetPiece = BoardManager.Instance.GetPieceAt(targetPos);
        if (targetPiece != null) return targetPiece.MyTeam != this.MyTeam;

        return true;
    }
}
