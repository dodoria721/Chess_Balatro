using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera _mainCamera;

    private GameObject selectedPiece; // 현재 선택되어 있는 기물(클릭 이동용)
    private GameObject draggedPiece;  // 현재 드래그 중인 기물(드래그 이동용)

    private Vector3 dragOffset;
    private Vector3 _lastValidPosition;
    private bool _hasMoved;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask pieceLayer; // 기물 레이어 (Inspector에서 Piece 레이어 선택)
    [SerializeField] private LayerMask tileLayer;  // 타일 레이어 (Inspector에서 Tile 레이어 선택)

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
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

    //public void Onclick(InputAction.CallbackContext context)
    //{
    //    if (context.started)
    //    {
    //        var rayHit = Physics2D.GetRayIntersection(
    //            _mainCamera.ScreenPointToRay(GetPointerScreenPosition()),
    //            Mathf.Infinity,
    //            pieceLayer
    //        );

    //        if (rayHit.collider != null)
    //        {
    //            selectedPiece = rayHit.collider.gameObject;
    //            draggedPiece = selectedPiece;
    //            _lastValidPosition = selectedPiece.transform.position;
    //            dragOffset = selectedPiece.transform.position - GetPointerWorldPosition();
    //            _hasMoved = false;
    //        }
    //        else
    //        {
    //            if (selectedPiece != null)
    //            {
    //                var tileHit = Physics2D.OverlapPoint(GetPointerWorldPosition(), tileLayer);
    //                if (tileHit != null)
    //                {
    //                    selectedPiece.transform.position = tileHit.transform.position;
    //                    ClearSelection();
    //                }
    //                else
    //                {
    //                    ClearSelection();
    //                }
    //            }
    //        }
    //    }
    //    else if (context.canceled)
    //    {
    //        if (draggedPiece != null)
    //        {
    //            if (_hasMoved)
    //            {
    //                HandleDrop();
    //            }
    //            draggedPiece = null;
    //        }
    //    }
    //}

    public void Onclick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var rayHit = Physics2D.GetRayIntersection(
                _mainCamera.ScreenPointToRay(GetPointerScreenPosition()),
                Mathf.Infinity,
                pieceLayer
            );

            if (rayHit.collider != null)
            {
                selectedPiece = rayHit.collider.gameObject;
                draggedPiece = selectedPiece;
                _lastValidPosition = selectedPiece.transform.position;
                dragOffset = selectedPiece.transform.position - GetPointerWorldPosition();
                _hasMoved = false;
                ShowValidMoves(selectedPiece); // 하이라이트 시작
            }
            else
            {
                if (selectedPiece != null)
                {
                    // 클릭으로 이동 시도 시 타일 감지
                    var tileHit = Physics2D.OverlapPoint(GetPointerWorldPosition(), tileLayer);
                    if (tileHit != null && tileHit.TryGetComponent(out Tile targetTile))
                    {
                        // === [검증 로직 추가] ===
                        if (selectedPiece.TryGetComponent(out PieceController controller))
                        {
                            if (controller.IsValidMove(targetTile.gridPos))
                            {
                                selectedPiece.transform.position = tileHit.transform.position;
                                controller.OnMoveConfirmed(targetTile.gridPos); // 상태 업데이트
                                ClearSelection();
                            }
                            else
                            {
                                // 규칙에 맞지 않으면 선택만 해제 (혹은 유지)
                                ClearSelection();
                            }
                        }
                    }
                    else
                    {
                        ClearSelection();
                    }
                }
            }
        }
        else if (context.canceled)
        {
            if (draggedPiece != null)
            {
                if (_hasMoved)
                {
                    HandleDrop();
                }
                // 드래그가 끝났을 때 draggedPiece는 null로 만들지만, 
                // 클릭 이동을 위해 selectedPiece는 HandleDrop 내부에서 상황에 따라 처리됩니다.
                draggedPiece = null;
            }
        }
    }


    //private void HandleDrop()
    //{
    //    float detectionRadius = 0.8f;
    //    var hit = Physics2D.OverlapCircle(GetPointerWorldPosition(), detectionRadius, tileLayer);

    //    if (hit != null)
    //    {
    //        draggedPiece.transform.position = hit.transform.position;
    //    }
    //    else
    //    {
    //        draggedPiece.transform.position = _lastValidPosition;
    //    }

    //    ClearSelection();
    //}

    private void HandleDrop()
    {
        float detectionRadius = 0.8f;
        var hit = Physics2D.OverlapCircle(GetPointerWorldPosition(), detectionRadius, tileLayer);

        // === [드래그 드롭 검증 로직 적용] ===
        if (hit != null && hit.TryGetComponent(out Tile targetTile))
        {
            if (draggedPiece.TryGetComponent(out PieceController controller))
            {
                // 기물에게 이 타일로 갈 수 있는지 물어봄
                if (controller.IsValidMove(targetTile.gridPos))
                {
                    draggedPiece.transform.position = hit.transform.position;
                    controller.OnMoveConfirmed(targetTile.gridPos); // 상태 업데이트(isFirstMove 등)
                }
                else
                {
                    // 규칙 위반 시 원래 자리로 복귀
                    draggedPiece.transform.position = _lastValidPosition;
                }
            }
        }
        else
        {
            // 타일이 없는 곳에 놓으면 복귀
            draggedPiece.transform.position = _lastValidPosition;
        }

        ClearSelection();
    }
    private void ShowValidMoves(GameObject piece)
    {
        if (piece.TryGetComponent(out PieceController controller))
        {
            // 씬에 있는 모든 타일을 찾습니다. 
            // (성능을 위해 보드매니저에 타일 리스트를 만들어두고 쓰는 게 더 좋지만, 일단 디버깅용으로 작성합니다.)
            Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);

            foreach (Tile tile in allTiles)
            {
                // 기물의 이동 규칙에 맞는지 확인
                if (controller.IsValidMove(tile.gridPos))
                {
                    tile.SetHighlight(true); // 갈 수 있는 곳이면 불 켜기
                }
            }
        }
    }

    private void HideValidMoves()
    {
        Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in allTiles)
        {
            tile.SetHighlight(false); // 모든 타일 불 끄기
        }
    }

    //private void ClearSelection()
    //{
    //    selectedPiece = null;
    //    draggedPiece = null;
    //    _hasMoved = false;
    //}
    private void ClearSelection()
    {
        HideValidMoves(); // 선택 해제 시 하이라이트 끄기
        selectedPiece = null;
        draggedPiece = null;
        _hasMoved = false;
    }

    //마우스와 터치 입력을 통합하여 화면 좌표를 반환
    //private Vector2 GetPointerScreenPosition()
    //{
    //    if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
    //    {
    //        return Touchscreen.current.primaryTouch.position.ReadValue();
    //    }
    //    return Mouse.current.position.ReadValue();
    //}
    private Vector2 GetPointerScreenPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        return Mouse.current.position.ReadValue();
    }

    //화면 좌표를 월드 좌표로 변환
    //private Vector3 GetPointerWorldPosition()
    //{
    //    Vector3 pos = GetPointerScreenPosition();
    //    pos.z = 10f;
    //    return _mainCamera.ScreenToWorldPoint(pos);
    //}

    private Vector3 GetPointerWorldPosition()
    {
        Vector3 pos = GetPointerScreenPosition();
        pos.z = 10f;
        return _mainCamera.ScreenToWorldPoint(pos);
    }
}