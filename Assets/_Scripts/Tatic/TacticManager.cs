using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TacticManager : MonoBehaviour
{
    public static TacticManager Instance;

    [Header("Owned Tatic")]
    // 유니티 인스펙터에서 리스트에 모든 전술 SO를 드래그해서 넣어주세요.
    [SerializeField] private List<TacticScriptableObject> allTactics = new List<TacticScriptableObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 현재 보드 상황을 분석하여 발생한 모든 전술의 [점수 합]과 [배수 합]을 반환합니다.
  
    public (float scoreSum, float multSum) GetCurrentTacticBonus()
    {
        float totalScore = 0f;
        float totalMult = 0f;

        // 1. 공격(Attack) 판정
        if (CheckAttack()) { AddBonus("Attack", ref totalScore, ref totalMult); }

        // 2. 더블 어택(Double Attack) 판정
        if (CheckDoubleAttack()) { AddBonus("Double Attack", ref totalScore, ref totalMult); }

        // 3. 포크(Fork) 판정
        if (CheckFork()) { AddBonus("Fork", ref totalScore, ref totalMult); }

        // ... 필요한 전술 판정 함수를 계속 추가 ...

        return (totalScore, totalMult);
    }

    // 보너스 합산을 도와주는 유틸리티 함수
    private void AddBonus(string name, ref float score, ref float mult)
    {
        var data = allTactics.FirstOrDefault(t => t.TacticName == name);
        if (data != null)
        {
            score += data.BaseScore;
            mult += data.MultValue;
            Debug.Log($"[전술 발동] {data.TacticName} : +{data.BaseScore}점 / x{data.MultValue}배");
        }
    }

    // --- [중요] 실제 보드 판정 로직 (알고리즘 구현부) ---
    // BoardManager.Instance.piecePositions 등을 활용하여 구현해야 합니다.

    private bool CheckAttack()
    {
        // 예: 아군 기물 중 하나라도 적을 공격 범위에 두고 있는가?
        return false; // 구현 필요
    }

    private bool CheckDoubleAttack()
    {
        // 예: 적 기물 하나를 아군 기물 둘 이상이 조준 중인가?
        return false; // 구현 필요
    }

    private bool CheckFork()
    {
        // 예: 아군 기물 하나가 적 기물 둘 이상을 조준 중인가?
        return false; // 구현 필요
    }
}