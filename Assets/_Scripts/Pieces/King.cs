using UnityEngine;

public class King : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        // 1. 기본 방향 및 아군 체크
        if (!base.IsValidMove(targetPos)) return false;

        // 2. 킹은 거리(절댓값 차이)가 1 이하여야 함 (대각선 포함)
        Vector2Int diff = targetPos - currentGridPos;
        if (Mathf.Abs(diff.x) > 1 || Mathf.Abs(diff.y) > 1) return false;

        return true;
    }
}