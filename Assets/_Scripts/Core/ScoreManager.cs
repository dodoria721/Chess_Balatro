using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void CalculateTurnScore(List<PieceController> movedPieces)
    {
        // 1. 보드 위의 아군 기물 기본 점수 합산
        float boardSum = BoardManager.Instance.piecePositions.Values
            .Where(p => p != null && p.MyTeam == Team.White)
            .Sum(p => p.pieceData.PieceScore);

        // 2. 전술 및 유물 보너스 가져오기
        var tactic = TacticManager.Instance.GetCurrentTacticBonus(movedPieces);
        var relic = RelicManager.Instance.GetRelicScoreBonus();

        // 3. 공식에 따른 최종 점수 계산
        float totalPlus = boardSum + tactic.scoreSum + alphaValue + relic.scoreSum;
        float totalMult = Mathf.Max(1f, tactic.multSum + relic.multSum);

        float finalScore = totalPlus * totalMult;

        // 4. 누적 점수에 합산
        _totalAccumulatedScore += finalScore;

        // [핵심 추가] UI 매니저에게 계산된 누적 점수를 전달하여 화면을 갱신합니다.
        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.RefreshScore(_totalAccumulatedScore);
        }

        Debug.Log($"<color=white>최종 점수: {finalScore} (누적 합계: {_totalAccumulatedScore})</color>");
    }
}