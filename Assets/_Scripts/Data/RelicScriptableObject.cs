using UnityEngine;

public enum RelicTier { Common, Rare, Legendary }

public enum RelicTriggerType
{
    OnTurnStart,       // 매 턴 시작 시
    OnTurnEnd,         // 매 턴 종료 시
    OnStageWin,        // 스테이지 승리
    OnCalculateScore,  // 점수 산출 시
    OnUpdatePerSecond  // 매 시간
}

[CreateAssetMenu(fileName = "RelicData", menuName = "Scriptable Objects/RelicScriptableObject")]
public class RelicScriptableObject : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string relicName;
    public string RelicName => relicName;

    [SerializeField][TextArea(3, 5)] private string description;

    // {PV}, {MV}, {SV}를 실제 수치로 바꿔서 반환하는 프로퍼티
    public string Description
    {
        get
        {
            string finalDesc = description;
            finalDesc = finalDesc.Replace("{PV}", plusValue.ToString());
            finalDesc = finalDesc.Replace("{MV}", multValue.ToString());
            finalDesc = finalDesc.Replace("{SV}", specialValue.ToString());
            return finalDesc;
        }
    }

    [SerializeField] private RelicTier relicTier;
    public RelicTier RelicTier => relicTier;

    [SerializeField] private Sprite relicIcon;
    public Sprite RelicIcon => relicIcon;

    [Header("Effect Settings")]
    [SerializeField] private RelicTriggerType triggerType;
    public RelicTriggerType TriggerType => triggerType;

    [Tooltip("기본 점수에 더해지는 값 (Chips)")]
    [SerializeField] private float plusValue;
    public float PlusValue { get => plusValue; set => plusValue = value; }

    [Tooltip("배수 연산에 사용되는 값 (Mult)")]
    [SerializeField] private float multValue;
    public float MultValue { get => multValue; set => multValue = value; }

    [Tooltip("체크 시 곱셈(x) 연산, 해제 시 덧셈(+) 연산")]
    [SerializeField] private bool isMultiplicative;
    public bool IsMultiplicative => isMultiplicative;

    [Tooltip("점수 외의 수치 (골드 수급량, AP 추가량 등)")]
    [SerializeField] private float specialValue;
    public float SpecialValue { get => specialValue; set => specialValue = value; }
}