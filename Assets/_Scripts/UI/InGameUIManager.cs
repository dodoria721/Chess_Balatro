using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;

    [Header("UI Components")]
    [SerializeField] private APDisplayUI apDisplay;
    [SerializeField] private Button turnEndButton;
    [SerializeField] private TextMeshProUGUI scoreText; // 유니티 에디터에서 점수 텍스트 연결

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // TurnManager가 호출하는 통로
    public void RefreshAP(int current, int baseMax, bool isNewTurn)
    {
        if (apDisplay != null)
            apDisplay.UpdateAP(current, baseMax, isNewTurn);
    }

    // 턴 종료 버튼 활성/비활성 제어
    public void SetTurnEndButtonInteractable(bool interactable)
    {
        if (turnEndButton != null)
            turnEndButton.interactable = interactable;
    }
    public void RefreshScore(float currentScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore:N0}"; // N0는 천단위 콤마 표시
        }
    }
}