using UnityEngine;

public class Queen : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        // 1. 방향 및 아군 존재 여부 체크
        if (!base.IsValidMove(targetPos)) return false;

        // 2. 경로가 비어있는지 체크 (퀸은 무한 기물이므로 항상 체크)
        if (!IsPathClear(targetPos)) return false;

        return true;
    }
}