using UnityEngine;
using TMPro;

public class APDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI apText;
    private int _currentTurnMaxAP;

    // UI를 실제로 그리는 부분 (어떻게 보여줄 것인가)
    public void UpdateAP(int current, int baseMax, bool isNewTurn)
    {
        if (apText == null) apText = GetComponent<TextMeshProUGUI>();

        // 새 턴 시작 시에만 유물 등이 반영된 현재 값을 최대치로 고정
        if (isNewTurn)
        {
            _currentTurnMaxAP = Mathf.Max(current, baseMax);
        }

        apText.text = $"AP: {current} / {_currentTurnMaxAP}";

        // 시각적 피드백: AP가 없으면 빨간색으로 표시
        apText.color = (current <= 0) ? Color.red : Color.white;
    }
}