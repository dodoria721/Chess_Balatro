using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GlossaryManager : MonoBehaviour
{
    [Header("화면 패널")]
    public GameObject categoryPanel;
    public GameObject gridPanel;

    [Header("격자 및 페이징 설정")]
    public Transform gridContent;
    public GameObject itemPrefab;
    public int itemsPerPage = 12;

    [Header("페이지 UI 연결")]
    public Button prevButton;
    public Button nextButton;
    public TextMeshProUGUI pageText;

    [Header("데이터 리스트 (기물 예시)")]
    public List<PieceScriptableObject> pieceDataList;

    private List<PieceScriptableObject> currentDisplayList;
    private int currentPage = 0;
    private int maxPage = 0;

    private void Start()
    {
        ShowCategoryPanel();
    }

    public void ShowCategoryPanel()
    {
        gridPanel.SetActive(false);
        categoryPanel.SetActive(true);
    }

    public void OnPiecesCategoryClicked()
    {
        OpenGridPage(pieceDataList);
    }

    private void OpenGridPage(List<PieceScriptableObject> dataList)
    {
        categoryPanel.SetActive(false);
        gridPanel.SetActive(true);

        currentDisplayList = dataList;
        currentPage = 0;

        maxPage = Mathf.CeilToInt((float)currentDisplayList.Count / itemsPerPage) - 1;
        if (maxPage < 0) maxPage = 0;

        UpdatePageDisplay();
    }

    private void UpdatePageDisplay()
    {
        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }

        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, currentDisplayList.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, gridContent);
            GlossaryItemUI itemUI = newItem.GetComponent<GlossaryItemUI>();

            if (itemUI != null)
            {
                itemUI.Setup(currentDisplayList[i]);
            }
        }

        pageText.text = $"{currentPage + 1} / {maxPage + 1}";

        prevButton.interactable = (currentPage > 0);
        nextButton.interactable = (currentPage < maxPage);
    }

    public void OnNextPageClicked()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            UpdatePageDisplay();
        }
    }

    public void OnPrevPageClicked()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePageDisplay();
        }
    }
}
