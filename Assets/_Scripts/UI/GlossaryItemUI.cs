using UnityEngine;
using TMPro;

public class GlossaryItemUI : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void Setup(PieceScriptableObject pieceData)
    {
        nameText.text = pieceData.PieceName;
        scoreText.text = "점수: " + pieceData.PieceScore.ToString();
    }
}
