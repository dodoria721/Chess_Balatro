using UnityEngine;
using System.Collections;

public enum GameState { PlayerTurn, EnemyTurn, Busy }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public GameState currentState { get; private set; }

    [Header("AP Settings")]
    public int maxAP = 3;
    public int currentAP { get; private set; }

    [SerializeField] private EnemyAIManager enemyAI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        currentState = GameState.PlayerTurn;
        currentAP = maxAP;
    }

    private void Start()
    {
        // 턴 시작 유물 효과 실행
        RelicManager.Instance?.ExecuteRelicEffects(RelicTriggerType.OnTurnStart);
        InGameUIManager.Instance?.RefreshAP(currentAP, maxAP, true);
    }

    public bool TryUseAP(int amount)
    {
        if (currentAP >= amount)
        {
            currentAP -= amount;
            InGameUIManager.Instance?.RefreshAP(currentAP, maxAP, false);
            return true;
        }
        return false;
    }

    public void OnTurnEndButtonClicked()
    {
        // 플레이어 턴일 때만 종료 버튼 작동
        if (currentState == GameState.PlayerTurn)
            StartCoroutine(SwitchToEnemyTurn());
    }

    private IEnumerator SwitchToEnemyTurn()
    {
        currentState = GameState.Busy;
        InGameUIManager.Instance?.SetTurnEndButtonInteractable(false);

        // 1. 플레이어 턴 종료 시 발동하는 유물 효과 (계산 전)
        RelicManager.Instance?.ExecuteRelicEffects(RelicTriggerType.OnTurnEnd);
        yield return new WaitForSeconds(0.3f);

        // 2. 적의 차례 진행 (여기서 내 기물이 파괴될 수 있음)
        currentState = GameState.EnemyTurn;
        yield return StartCoroutine(enemyAI.PlayTurn());

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CalculateTurnScore();
        }

        // 4. 다시 플레이어 턴으로 복귀 준비
        currentAP = maxAP;

        // 새 턴 시작 시 유물 효과 (예: 추가 AP 등)
        RelicManager.Instance?.ExecuteRelicEffects(RelicTriggerType.OnTurnStart);

        InGameUIManager.Instance?.RefreshAP(currentAP, maxAP, true);
        InGameUIManager.Instance?.SetTurnEndButtonInteractable(true);

        currentState = GameState.PlayerTurn;
    }

    public void AddCurrentAP(int amount)
    {
        currentAP += amount;
        InGameUIManager.Instance?.RefreshAP(currentAP, maxAP, false);
    }
}