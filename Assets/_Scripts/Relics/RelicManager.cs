using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Sum, Where 사용을 위해 필요

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;

    [Header("Owned Relics")]
    [SerializeField] private List<RelicScriptableObject> list_ownedRelics = new List<RelicScriptableObject>();

    [Header("Score Settings")]
    [SerializeField] private float alphaValue = 0f; // 공식의 'a' (추가 점수)
    private float _totalAccumulatedScore = 0f;    // 누적 점수

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// 매 턴 종료 시 호출하여 보드 상황을 체크하고 점수를 산출 및 누적합니다.
    /// 공식: (기물 점수 합 + 전술 점수 + a + 유물 합산) * (기물 배수 + 전술 배수 + 유물 배수 합)
    /// </summary>
    public void CalculateAndAccumulateTurnScore()
    {
        // 1. 현재 보드의 아군(White) 기물들의 기본 점수 실시간 합산
        float boardPieceScoreSum = BoardManager.Instance.piecePositions.Values
            .Where(p => p != null && p.MyTeam == Team.White)
            .Sum(p => p.pieceData.PieceScore);

        // 2. 미구현 수치들 (전술 점수, 기물 배수 등 - 필요 시 외부에서 전달받도록 수정 가능)
        float tacticScore = 0f;   // 전술 점수
        float tacticMult = 0f;    // 전술 배수
        float pieceMultSum = 0f;  // 기물 배수

        // 3. 유물로부터 합산 점수(PlusValue)와 합산 배수(MultValue) 추출
        float relicPlusSum = 0f;
        float relicMultSum = 0f;

        foreach (var relic in list_ownedRelics)
        {
            if (relic.TriggerType == RelicTriggerType.OnCalculateScore)
            {
                relicPlusSum += relic.PlusValue;
                // 공식상 '배수들의 합'이므로 IsMultiplicative 체크 여부와 상관없이 합산
                relicMultSum += relic.MultValue;
            }
        }

        // 4. 최종 공식 적용
        // 기본 배수를 1로 잡을지 0으로 잡을지는 기획에 따라 다르지만, 
        // 일반적으로 (배수 합)이 0이면 점수가 0이 되므로 기본값 1을 더해주는 경우가 많습니다.
        float totalMult = pieceMultSum + tacticMult + relicMultSum;
        if (totalMult <= 0) totalMult = 1f; // 배수가 없으면 최소 1배

        float turnScore = (boardPieceScoreSum + tacticScore + alphaValue + relicPlusSum) * totalMult;

        // 5. 점수 누적 및 UI 업데이트
        _totalAccumulatedScore += turnScore;
        InGameUIManager.Instance?.RefreshScore(_totalAccumulatedScore);

        Debug.Log($"[Score Calculation] 보드기물:{boardPieceScoreSum} + 유물합산:{relicPlusSum} | 배수합:{totalMult} | 이번턴:{turnScore} | 누적:{_totalAccumulatedScore}");
    }

    // --- 기존 유물 효과 실행 로직 유지 ---
    public void ExecuteRelicEffects(RelicTriggerType targetTrigger)
    {
        foreach (var relic in list_ownedRelics)
        {
            if (relic.TriggerType == targetTrigger)
            {
                ApplyRelicEffect(relic);
            }
        }
    }

    private void ApplyRelicEffect(RelicScriptableObject relic)
    {
        switch (relic.RelicName)
        {
            case "Gambit":
                ApplyGambit(relic);
                break;
        }
    }

    private void ApplyGambit(RelicScriptableObject relic)
    {
        int bonusAP = (int)relic.SpecialValue;
        TurnManager.Instance.AddCurrentAP(bonusAP);
        Debug.Log($"[Relic] {relic.RelicName} 발동: +{bonusAP} AP");
    }
}