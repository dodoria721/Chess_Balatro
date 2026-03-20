using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

// IPointerEnterHandler, IPointerExitHandler를 상속받ㅇㅏ 마우스 호버 이베ㄴㅌㅡ 감ㅈㅣ
public class HandPieceUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Piece Data")]
    public PieceScriptableObject pieceData; // 이 UI가 어ㄸㅓㄴ 기무ㄹㅇㅣㄴ지 연ㄱㅕㄹ

    [Header("Hover Settings")]
    public float hoverScaleMultiplier = 1.2f; // 호버 시 커지ㄹ 비유ㄹ
    private Vector3 _originalScale;

    private Coroutine _tooltipCoroutine;

    private void Start()
    {
        _originalScale = transform.localScale; // 원ㄹㅐ 크기 저자ㅇ
    }

    // 마우스가 UI 위에 올ㄹㅏㅇㅘㅆ을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = _originalScale * hoverScaleMultiplier;

        if (_tooltipCoroutine != null) StopCoroutine(_tooltipCoroutine);
        _tooltipCoroutine = StartCoroutine(ShowTooltipRoutine());
    }

    // 마우스가 UI 밖ㅇㅡㄹㅗ 나가ㅆㅇㅡㄹ 때
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = _originalScale;

        if (_tooltipCoroutine != null) StopCoroutine(_tooltipCoroutine);
        TooltipManager.Instance.HideTooltip();
    }

    private IEnumerator ShowTooltipRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        if (pieceData != null)
        {
            TooltipManager.Instance.ShowTooltip(pieceData.PieceName, pieceData.Description);
        }
    }
}
