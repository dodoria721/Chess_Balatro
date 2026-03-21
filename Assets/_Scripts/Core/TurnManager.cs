using UnityEngine;
using System.Collections;
using TMPro; // UI 표시를 위해 필요

public enum GameState { PlayerTurn, EnemyTurn, Busy }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public GameState currentState { get; private set; }

    [Header("AP Settings")]
    public int maxAP = 3;      // 턴당 최대 행동력
    public int currentAP { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI apText; // Inspector에서 Text 오브젝트 연결

    [SerializeField] private EnemyAIManager enemyAI;

    void Awake()
    {
        if (Instance == null) Instance = this;
        currentState = GameState.PlayerTurn;
        currentAP = maxAP;
        UpdateAPUI();
    }

    // 기물 이동 시 호출하여 AP 소비
    public bool TryUseAP(int amount)
    {
        if (currentAP >= amount)
        {
            currentAP -= amount;
            UpdateAPUI();
            Debug.Log($"AP 소비! 남은 AP: {currentAP}");
            return true;
        }
        Debug.Log("행동력이 부족합니다.");
        return false;
    }

    // UI 버튼(Turn End)에 연결할 함수
    public void OnTurnEndButtonClicked()
    {
        if (currentState == GameState.PlayerTurn)
        {
            StartCoroutine(SwitchToEnemyTurn());
        }
    }

    private IEnumerator SwitchToEnemyTurn()
    {
        currentState = GameState.Busy;
        yield return new WaitForSeconds(0.3f);

        currentState = GameState.EnemyTurn;
        yield return StartCoroutine(enemyAI.PlayTurn());

        // 다시 플레이어 턴으로 복귀 시 AP 회복
        currentAP = maxAP;
        UpdateAPUI();
        currentState = GameState.PlayerTurn;
        Debug.Log("플레이어 턴 시작! AP가 충전되었습니다.");
    }

    private void UpdateAPUI()
    {
        if (apText != null)
        {
            apText.text = $"AP: {currentAP} / {maxAP}";
        }
    }
}