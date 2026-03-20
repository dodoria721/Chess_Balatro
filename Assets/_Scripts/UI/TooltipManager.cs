using UnityEngine;
using TMPro; //TextMeshPro 쓸까?

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [Header("UI References")]
    public GameObject tooltipPanel; // 툴팁 배경 패널
    public TextMeshProUGUI nameText; // 이름 텍스트
    public TextMeshProUGUI descriptionText; // 설명 텍스트

    private void Awake()
    {
        if (Instance == null) Instance = this;
        HideTooltip();
    }

    private void Update()
    {
        // 툴팁이 켜져 있을 때 마우스를 따라 다니게 하려면 주석 제거.
        // if (tooltipPanel.activeSelf)
        // {
        //     tooltipPanel.transform.position = Input.mousePosition;
        // }
    }

    public void ShowTooltip(string pieceName, string description)
    {
        nameText.text = pieceName;
        descriptionText.text = description;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
