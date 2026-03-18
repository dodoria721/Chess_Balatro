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
            Vector2Int detectedPos = BoardManager.Instance.WorldToGridPos(transform.position);
            BoardManager.Instance.UpdatePiecePosition(currentGridPos, detectedPos, this);
            currentGridPos = detectedPos;
        }
    }

    // 해당 타일에 접근 가능한지?
    public virtual bool IsValidMove(Vector2Int targetPos)
    {
        if (targetPos == currentGridPos) return false; // 가고자 하는 곳과 내가 있는 곳이 같다 -> 이동할 수 없다.
        Vector2Int diff = targetPos - currentGridPos; // 가고자 하는 곳과 내가 있는 곳의 차, 갈 수 있는 곳인지 체크 하기 위한 값

        foreach (Vector2Int dir in currentMoveDirections)
        {
            if (currentIsInfinity) // 무한이 이동가능한 기물(퀸, 비숍, 룩)
            {
                if (IsMovingInDirection(diff, dir)) return true; // 가고자 하는 벡터값과, 갈 수 있는 벡터 값이 같다 -> 이동할 수 있다.
            }
            else if (diff == dir) return true; // 칸 이 정해져 있는 기물(킹, 나이트, 폰)
        }
        return false;
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

    public virtual void OnMoveConfirmed(Vector2Int targetPos)
    {
        Vector2Int oldPos = currentGridPos;
        BoardManager.Instance.UpdatePiecePosition(currentGridPos, targetPos, this);
        currentGridPos = targetPos;
        isFirstMove = false;
    }
}