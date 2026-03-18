using UnityEngine;

[CreateAssetMenu(fileName = "PieceScriptableObject", menuName = "Scriptable Objects/PieceScriptableObject")]
public class PieceScriptableObject : ScriptableObject
{
    [SerializeField]
    string pieceName;
    public string PieceName { get => pieceName; private set => pieceName = value; }

    [SerializeField]
    float pieceScore;
    public float PieceScore { get => pieceScore; private set => pieceScore = value; }


    [Header("Movement Settings")]
    [SerializeField]
    Vector2Int[] moveDirections;
    public Vector2Int[] MoveDirections { get => moveDirections; private set => moveDirections = value; }

    [SerializeField]
    bool isInfinite;
    public bool IsInfinite { get => isInfinite; private set => isInfinite = value; }
}
