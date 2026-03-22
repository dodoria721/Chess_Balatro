using UnityEngine;
using System.Collections.Generic;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;

    [Header("Owned Relics")]
    [SerializeField] private List<RelicScriptableObject> list_ownedRelics = new List<RelicScriptableObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    /// <summary>
    /// ScoreManager에서 호출하여 유물로 인한 점수 합산치와 배수 합산치를 가져옵니다.
    /// </summary>
    public (float scoreSum, float multSum) GetRelicScoreBonus()
    {
        float scoreSum = 0f;
        float multSum = 0f;

        foreach (var relic in list_ownedRelics)
        {
            if (relic.TriggerType == RelicTriggerType.OnCalculateScore)
            {
                scoreSum += relic.PlusValue;
                multSum += relic.MultValue;
            }
        }
        return (scoreSum, multSum);
    }

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