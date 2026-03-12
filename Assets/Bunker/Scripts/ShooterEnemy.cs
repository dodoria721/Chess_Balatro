using UnityEngine;

public class ShooterEnemy : MonsterMove
{
    [Header("PoolManager에 할당된 프리팹 아이디")]
    public int prefabId = 0;

    public float speed = 0f;
    public float damage = 0f;
    public float rateOfFire = 3f;
    public float attackRange = 10f;
    private float fireTimer;

    protected override void Start()
    {
        base.Start();
        fireTimer = 0f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        fireTimer += Time.fixedDeltaTime;
        if (fireTimer >= rateOfFire)
        {
            fireTimer = 0f;
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        GameObject newRangeObject = GameManager.Instance.pool.Get(prefabId);
        if (newRangeObject == null)
        {
            Debug.LogError("오브젝트 풀에서 총알을 가져오는 데 실패했습니다.");
            return;
        }

        IRange range = newRangeObject.GetComponent<IRange>();
        Transform rangeTransform = newRangeObject.transform;

        // 총알이 향할 방향을 계산
        Vector3 dir = (playerTransform.position - transform.position).normalized;

        rangeTransform.position = transform.position;
        rangeTransform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        range.Init(speed, damage, 0, dir, "Enemy");
    }
}