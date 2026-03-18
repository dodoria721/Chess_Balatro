using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPos;
    public SpriteRenderer spriteRenderer;

    // 새로 추가
    private Color _originalColor;
    private bool _isColorSet = false;

    public void SetColor(bool isOffset)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        _originalColor = isOffset ? new Color(0.8f, 0.8f, 0.8f, 1f) : Color.white;
        spriteRenderer.color = _originalColor;
        _isColorSet = true;
    }

    // 기존 SetColor
    //public void SetColor(bool isOffset)
    //{
    //    spriteRenderer.color = isOffset ? new Color(0.8f, 0.8f, 0.8f) : Color.white;
    //}

    // 새로 추가
    public void SetHighlight(bool active)
    {
        if (spriteRenderer == null || !_isColorSet) return;

        if (active)
        {
            spriteRenderer.color = new Color(1f, 1f, 0f, 1f);
        }
        else
        {
            spriteRenderer.color = _originalColor;
        }
    }
}