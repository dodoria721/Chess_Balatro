using UnityEngine;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Settings")]
    [SerializeField] private float alphaValue = 0f; // 공식의 'a'
    private float _totalAccumulatedScore = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void CalculateTurnScore()
    {
        // 1. 보드 기물 기본 점수 (BoardManager에서 가져옴)
        float boardSum = BoardManager.Instance.piecePositions.Values
            .Where(p => p != null && p.MyTeam == Team.White)
            .Sum(p => p.pieceData.PieceScore);

        // 2. 전술 매니저 보너스 (TacticManager에서 가져옴)
        var tactic = TacticManager.Instance.GetCurrentTacticBonus();

        // 3. 유물 매니저 보너스 (RelicManager에서 가져옴)
        var relic = RelicManager.Instance.GetRelicScoreBonus();

        // 4. 공식 적용: (기물 합 + 전술 점수 + a + 유물 점수) * (전술 배수 + 유물 배수)
        float totalPlus = boardSum + tactic.scoreSum + alphaValue + relic.scoreSum;
        float totalMult = tactic.multSum + relic.multSum;

        // 배수 보정 (0배 방지)
        if (totalMult <= 0) totalMult = 1f;

        float turnScore = totalPlus * totalMult;

        // 5. 누적 및 UI 반영
        _totalAccumulatedScore += turnScore;
        InGameUIManager.Instance?.RefreshScore(_totalAccumulatedScore);

        Debug.Log($"[Score] 턴 점수: {turnScore} | 총점: {_totalAccumulatedScore}");
    }
}