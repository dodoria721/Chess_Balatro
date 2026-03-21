using UnityEngine;

public struct Move
{
    public PieceController piece;       // 움직이는 기물
    public Vector2Int fromPos;          // 출발지
    public Vector2Int toPos;            // 목적지
    public PieceController capturedPiece; // 그 자리에 있어 잡히게 되는 기물
    public bool wasFirstMove;           // 첫 이동 여부 (Undo를 위해 저장)

    public Move(PieceController p, Vector2Int from, Vector2Int to, PieceController captured, bool firstMove)
    {
        piece = p;
        fromPos = from;
        toPos = to;
        capturedPiece = captured;
        wasFirstMove = firstMove;
    }
}