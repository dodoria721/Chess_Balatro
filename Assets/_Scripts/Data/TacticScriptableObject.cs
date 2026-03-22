using UnityEngine;

// 전술 등급 구분 (Basic, Advanced, Legendary 등)
public enum TacticTier { Basic, Advanced, Legendary }

[CreateAssetMenu(fileName = "TacticData", menuName = "Scriptable Objects/TacticScriptableObject")]
public class TacticScriptableObject : ScriptableObject
{
    [SerializeField]
    private string tacticName;
    public string TacticName => tacticName;

    [SerializeField]
    private float baseScore;    // 기본 점수
    public float BaseScore => baseScore;

    [SerializeField]
    private float multValue;    // 배수 (Mult)
    public float MultValue => multValue;

    [SerializeField]
    [TextArea] private string description;
    public string Description => description;

    [SerializeField]
    private TacticTier tier;
    public TacticTier Tier => tier;
}