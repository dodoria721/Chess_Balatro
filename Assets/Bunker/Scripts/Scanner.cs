using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public static Scanner Instance { get; private set; }

    // 원현으로 탐색하기위한 반지름 값
    [SerializeField] private float scanRange;
    public LayerMask targetLayer;
    public Transform nearestTarget;

    // 싱글톤 패턴을 위한 변수
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 코루틴을 이용한 스캔 주기 조절
    private void Start()
    {
        StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        while (true)
        {
            // 원하는 스캔 주기(초) 설정
            yield return new WaitForSeconds(0.2f);

            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, scanRange, targetLayer);
            nearestTarget = GetNearest(targets);

        }
    }

    public Transform GetNearest(Collider2D[] targets)
    {
        Transform result = null;
        float shortestSqrDistance = float.MaxValue;

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 targetPosition = targets[i].transform.position;
            float currentSqrDistance = (transform.position - targetPosition).sqrMagnitude;

            if (currentSqrDistance < shortestSqrDistance)
            {
                shortestSqrDistance = currentSqrDistance;
                result = targets[i].transform;
            }
        }
        return result;
    }
}