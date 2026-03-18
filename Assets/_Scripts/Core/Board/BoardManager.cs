using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board Settings")]
    public int width = 6;
    public int height = 6;
    public float tileSize = 1f; // 16f에서 1f로 조정 권장

    public GameObject tilePrefab;
    public Dictionary<Vector2Int, PieceController> piecePositions = new Dictionary<Vector2Int, PieceController>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        CreateBoard();
    }

    public void CreateBoard()
    {
        float startX = -(width * tileSize) / 2f + (tileSize / 2f);
        float startY = -(height * tileSize) / 2f + (tileSize / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPos = new Vector3(startX + (x * tileSize), startY + (y * tileSize), 0);
                GameObject tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{y}";

                if (tile.TryGetComponent(out Tile tileScript))
                {
                    tileScript.gridPos = new Vector2Int(x, y);
                    tileScript.SetColor((x + y) % 2 != 0);
                }
            }
        }
    }

    // 월드 좌표 -> 격자 좌표 변환(Piece의 초기 좌표를 통해 해당하는 tile의 위치를 알기 위해)
    public Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        float startX = -(width * tileSize) / 2f + (tileSize / 2f);
        float startY = -(height * tileSize) / 2f + (tileSize / 2f);

        int x = Mathf.RoundToInt((worldPos.x - startX) / tileSize);
        int y = Mathf.RoundToInt((worldPos.y - startY) / tileSize);
        return new Vector2Int(x, y);
    }

    public PieceController GetPieceAt(Vector2Int pos) => piecePositions.ContainsKey(pos) ? piecePositions[pos] : null;

    public void UpdatePiecePosition(Vector2Int oldPos, Vector2Int newPos, PieceController piece)
    {
        if (piecePositions.ContainsKey(oldPos) && piecePositions[oldPos] == piece) piecePositions.Remove(oldPos);
        piecePositions[newPos] = piece;
    }
}