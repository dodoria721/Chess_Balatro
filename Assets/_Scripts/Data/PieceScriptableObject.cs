using UnityEngine;

public enum Team { White, Black, Neutral} // 아군, 적, 중립(혹시 몰라 추가)

[CreateAssetMenu(fileName = "PieceScriptableObject", menuName = "Scriptable Objects/PieceScriptableObject")]
public class PieceScriptableObject : ScriptableObject
{
    [SerializeField]
    string pieceName;
    public string PieceName { get => pieceName; private set => pieceName = value; }

    [SerializeField]
    Team pieceTeam;
    public Team PieceTeam { get => pieceTeam; private set => pieceTeam = value; }

    [SerializeField]
    float pieceScore;
    public float PieceScore { get => pieceScore; private set => pieceScore = value; }

    [SerializeField, TextArea(3, 5)] //�⹰ ������ ����
    string description;
    public string Description { get => description; private set => description = value; }


    [Header("Movement Settings")]
    [SerializeField]
    Vector2Int[] moveDirections;
    public Vector2Int[] MoveDirections { get => moveDirections; private set => moveDirections = value; }

    [SerializeField]
    bool isInfinite;
    public bool IsInfinite { get => isInfinite; private set => isInfinite = value; }

}
