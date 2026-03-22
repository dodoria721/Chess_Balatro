using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera _mainCamera;
    private GameObject selectedPiece;
    private GameObject draggedPiece;
    private Vector3 dragOffset;
    private Vector3 _lastValidPosition;
    private bool _hasMoved;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask pieceLayer;
    [SerializeField] private LayerMask tileLayer;

    private void Awake() => _mainCamera = Camera.main;

    private void Update()
    {
        // 플레이어 턴이 아니면 조작 차단
        if (TurnManager.Instance.currentState != GameState.PlayerTurn) return;

        if (draggedPiece != null)
        {
            Vector3 pointerPos = GetPointerWorldPosition();
            Vector3 targetPos = pointerPos + dragOffset;
            targetPos.z = 0;

            if (Vector3.Distance(draggedPiece.transform.position, targetPos) > 0.01f)
            {
                _hasMoved = true;
                draggedPiece.transform.position = targetPos;
            }
        }
    }

    public void Onclick(InputAction.CallbackContext context)
    {
        if (TurnManager.Instance.currentState != GameState.PlayerTurn) return;

        if (context.started)
        {
            var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(GetPointerScreenPosition()), Mathf.Infinity, pieceLayer);

            if (rayHit.collider != null)
            {
                // 내 기물(White)일 때만 선택
                if (rayHit.collider.TryGetComponent(out PieceController piece) && piece.MyTeam == Team.White)
                {
                    HideValidMoves();
                    selectedPiece = rayHit.collider.gameObject;
                    draggedPiece = selectedPiece;
                    _lastValidPosition = selectedPiece.transform.position;
                    dragOffset = selectedPiece.transform.position - GetPointerWorldPosition();
                    _hasMoved = false;
                    ShowValidMoves(selectedPiece);
                }
            }
            else if (selectedPiece != null)
            {
                // 클릭 이동 시도
                AttemptMove(GetPointerWorldPosition());
            }
        }
        else if (context.canceled && draggedPiece != null)
        {
            if (_hasMoved) HandleDrop();
            draggedPiece = null;
        }
    }

    private void AttemptMove(Vector3 worldPos)
    {
        var tileHit = Physics2D.OverlapPoint(worldPos, tileLayer);
        if (tileHit != null && tileHit.TryGetComponent(out Tile targetTile))
        {
            if (selectedPiece.TryGetComponent(out PieceController controller))
            {
                // 규칙 확인 및 AP 1 이상 있는지 확인
                if (controller.IsValidMove(targetTile.gridPos) && TurnManager.Instance.currentAP > 0)
                {
                    if (TurnManager.Instance.TryUseAP(1))
                    {
                        selectedPiece.transform.position = tileHit.transform.position;
                        controller.OnMoveConfirmed(targetTile.gridPos);
                    }
                }
            }
        }
        ClearSelection();
    }

    private void HandleDrop()
    {
        float detectionRadius = 0.8f;
        var hit = Physics2D.OverlapCircle(GetPointerWorldPosition(), detectionRadius, tileLayer);

        if (hit != null && hit.TryGetComponent(out Tile targetTile) && draggedPiece.TryGetComponent(out PieceController controller))
        {
            if (controller.IsValidMove(targetTile.gridPos) && TurnManager.Instance.currentAP > 0)
            {
                if (TurnManager.Instance.TryUseAP(1))
                {
                    draggedPiece.transform.position = hit.transform.position;
                    controller.OnMoveConfirmed(targetTile.gridPos);

                    TurnManager.Instance.RecordMovement(controller);

                    ClearSelection();
                    return;
                }
            }
        }
        draggedPiece.transform.position = _lastValidPosition;
        ClearSelection();
    }

    private void ShowValidMoves(GameObject piece)
    {
        if (TurnManager.Instance.currentAP <= 0) return; // AP 없으면 하이라이트 안 함
        if (piece.TryGetComponent(out PieceController controller))
        {
            Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
            foreach (Tile tile in allTiles)
            {
                if (controller.IsValidMove(tile.gridPos)) tile.SetHighlight(true);
            }
        }
    }

    private void HideValidMoves()
    {
        Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in allTiles) tile.SetHighlight(false);
    }

    private void ClearSelection()
    {
        HideValidMoves();
        selectedPiece = null;
        draggedPiece = null;
        _hasMoved = false;
    }

    private Vector2 GetPointerScreenPosition() => Mouse.current.position.ReadValue();
    private Vector3 GetPointerWorldPosition() { Vector3 pos = GetPointerScreenPosition(); pos.z = 10f; return _mainCamera.ScreenToWorldPoint(pos); }
}