using UnityEngine;

public class Rook : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        // 1. 부모가 방향(궤도)과 아군 여부를 체크
        if (!base.IsValidMove(targetPos)) return false;

        // 2. 무한 기물 전용: 사이 경로에 장애물이 있는지 체크
        if (!IsPathClear(targetPos)) return false;

        return true;
    }

}
