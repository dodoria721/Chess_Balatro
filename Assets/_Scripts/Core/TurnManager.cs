using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState { PlayerTurn, EnemyTurn, Busy }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public GameState currentState { get; private set; }

    [Header("AP Settings")]
    public int maxAP = 3;
    public int currentAP { get; private set; }

    [SerializeField] private EnemyAIManager enemyAI;

    // 이번 턴에 이동한 기물을 추적하여 TacticManager에 전달하기 위함
    private List<PieceController> movedPiecesThisTurn = new List<PieceController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        currentState = GameState.PlayerTurn;
        currentAP = maxAP;
    }

    private void Start()
    {
        RelicManager.Instance?.ExecuteRelicEffects(RelicTriggerType.OnTurnStart);
        InGameUIManager.Instance?.RefreshAP(currentAP, maxAP, true);
    }

    // [중요] RelicManager에서 호출하는 함수
    public void AddCurrentAP(int amount)
    {
        currentAP += amount;
        // UI 업데이트도 함께 수행
        InGameUIManager.Instance?.RefreshAP(currentAP, maxAP, false);
    }

    public void RecordMovement(PieceController piece)
    {
        if (!movedPiecesThisTurn.Contains(piece))
            movedPiecesThisTurn.Add(piece);
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
        if (currentState == GameState.PlayerTurn)
            StartCoroutine(FinalizePlayerTurn());
    }

    private IEnumerator FinalizePlayerTurn()
    {
        currentState = GameState.Busy;
        InGameUIManager.Instance?.SetTurnEndButtonInteractable(false);

        Debug.Log("<color=yellow>--- 플레이어 턴 종료: 점수 정산 시작 ---</color>");

        if (ScoreManager.Instance != null)
        {
            // 이동한 기물 리스트를 넘겨줌
            ScoreManager.Instance.CalculateTurnScore(movedPiecesThisTurn);
        }

        yield return new WaitForSeconds(0.7f);

        RelicManager.Instance?.ExecuteRelicEffects(RelicTriggerType.OnTurnEnd);
        yield return new WaitForSeconds(0.3f);

        currentState = GameState.EnemyTurn;
        yield return StartCoroutine(enemyAI.PlayTurn());

        // 다음 턴 준비
        currentAP = maxAP;
        movedPiecesThisTurn.Clear(); // 이동 기록 초기화

        RelicManager.Instance?.ExecuteRelicEffects(RelicTriggerType.OnTurnStart);
        InGameUIManager.Instance?.RefreshAP(currentAP, maxAP, true);
        InGameUIManager.Instance?.SetTurnEndButtonInteractable(true);

        currentState = GameState.PlayerTurn;
        Debug.Log("<color=green>--- 새로운 플레이어 턴 시작 ---</color>");
    }
}