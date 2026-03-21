using UnityEngine;

public class PieceController : MonoBehaviour
{
    public PieceScriptableObject pieceData;
    protected Vector2Int currentGridPos; // 현재 기물이 위치한 보드의 (x,y) 값
    public bool isFirstMove = true;

    protected string currentPiecetName;
    protected float currentPieceScore;
    protected Vector2Int[] currentMoveDirections; // 기물이 이동할 수 있는 방향 벡터를 담은 배열
    protected bool currentIsInfinity; // 기물이 보드 끝까지 가는지 체크(퀸, 비숍, 룩)
    public Team MyTeam { get; private set; }

    void Awake()
    {
        if (pieceData != null)
        {
            MyTeam = pieceData.PieceTeam;
            currentPiecetName = pieceData.PieceName;
            currentPieceScore = pieceData.PieceScore;
            currentMoveDirections = pieceData.MoveDirections;
            currentIsInfinity = pieceData.IsInfinite;
        }
    }

    // 시작 시 자신의 위치를 자동으로 보드에 등록
    void Start()
    {
        if (BoardManager.Instance != null)
        {
            // 1. 현재 월드 좌표로 실제 격자 좌표를 먼저 구함
            Vector2Int detectedPos = BoardManager.Instance.WorldToGridPos(transform.position);

            // 2. 내 변수를 먼저 업데이트
            Vector2Int oldPos = currentGridPos; // 초기값은 보통 (0,0) 혹은 에디터 설정값
            currentGridPos = detectedPos;

            // 3. 그 다음 보드 매니저에 나 여기 있다고 신고
            BoardManager.Instance.UpdatePiecePosition(oldPos, currentGridPos, this);
        }
    }

    // 해당 타일에 접근 가능한지?
    public virtual bool IsValidMove(Vector2Int targetPos)
    {
        // 1. 보드 범위 밖이거나 제자리 이동 체크
        if (targetPos.x < 0 || targetPos.x >= BoardManager.Instance.width ||
            targetPos.y < 0 || targetPos.y >= BoardManager.Instance.height) return false;
        if (targetPos == currentGridPos) return false;

        // 2. 도착지에 아군 기물이 있는지 체크
        PieceController targetPiece = BoardManager.Instance.GetPieceAt(targetPos);
        if (targetPiece != null && targetPiece.MyTeam == this.MyTeam) return false;

        // 3. IsInfinity 체크 (기본 이동 궤적 확인)
        Vector2Int diff = targetPos - currentGridPos;
        bool isStepValid = false;
        foreach (Vector2Int dir in currentMoveDirections)
        {
            if (currentIsInfinity)
            {
                if (IsMovingInDirection(diff, dir)) { isStepValid = true; break; }
            }
            else
            {
                // 기본적으로 단거리 기물은 지정된 벡터와 정확히 일치해야 함
                if (diff == dir) { isStepValid = true; break; }
            }
        }

        return isStepValid;
    }

    // (킹, 비숍, 룩)의 가고자 하는 방향을 체크 하는 함수
    protected bool IsMovingInDirection(Vector2Int diff, Vector2Int dir)
    {
        if (dir.x != 0 && dir.y == 0 && diff.y == 0 && Mathf.Sign(diff.x) == Mathf.Sign(dir.x)) return true; // 가로 이동 체크
        if (dir.y != 0 && dir.x == 0 && diff.x == 0 && Mathf.Sign(diff.y) == Mathf.Sign(dir.y)) return true; // 세로 이동 체크
        if (dir.x != 0 && dir.y != 0 && Mathf.Abs(diff.x) == Mathf.Abs(diff.y) &&                            // 대각선 이동 체크
            Mathf.Sign(diff.x) == Mathf.Sign(dir.x) && Mathf.Sign(diff.y) == Mathf.Sign(dir.y)) return true;
        return false;
    }

    protected bool IsPathClear(Vector2Int targetPos)
    {
        Vector2Int diff = targetPos - currentGridPos;

        Vector2Int direction = new Vector2Int(
            diff.x == 0 ? 0 : (int)Mathf.Sign(diff.x),
            diff.y == 0 ? 0 : (int)Mathf.Sign(diff.y)
            );

        Vector2Int checkPos = currentGridPos + direction;

        while (checkPos != targetPos)
        {
            if (BoardManager.Instance.GetPieceAt(checkPos) != null) return false;

            checkPos += direction;
        }

        return true;
    }

    public virtual void OnMoveConfirmed(Vector2Int newPos)
    {
        Vector2Int oldPos = currentGridPos;
        currentGridPos = newPos;

        // 보드 데이터 갱신
        BoardManager.Instance.UpdatePiecePosition(oldPos, newPos, this);

        // [추가] 이동이 확인되었으므로 첫 이동 체크 해제
        if (isFirstMove) isFirstMove = false;

        // Debug.Log($"{currentPiecetName} 이동 완료: {newPos}");
    }

    public Vector2Int GetCurrentGridPos() => currentGridPos;
}