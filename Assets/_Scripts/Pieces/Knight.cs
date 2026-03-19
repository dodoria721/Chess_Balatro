using UnityEngine;

public class Knight : PieceController
{
    public override bool IsValidMove(Vector2Int targetPos)
    {
        // 부모가 "L자 방향인지(diff==dir)"와 "아군이 없는지"를 이미 체크함
        if (!base.IsValidMove(targetPos)) return false;

        // 나이트는 사이 경로를 검사할 필요가 없으므로 여기서 바로 true
        return true;
    }
}