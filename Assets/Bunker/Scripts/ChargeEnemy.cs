using UnityEngine;

public class ChargeEnemy : MonsterMove
{
    public enum MonsterState
    {
        Patrol,      // 순찰 (기본 이동)
        ChargeReady, // 돌진 준비
        Charging,    // 돌진
        Cooldown     // 쿨다운
    }

    [Header("돌진 공격")]
    [SerializeField] private float chargeSpeed = 10f;          // 돌진 속도
    [SerializeField] private float chargeDuration = 1.0f;      // 돌진 유지 시간(길수록 더 멀리 감)
    [SerializeField] private float chargeCooldown = 2.0f;      // 돌진 쿨타임
    [SerializeField] private float chargeRange = 15f;          // 몬스터 시아 범위

    public MonsterState currentState = MonsterState.Patrol;
    private float stateTimer = 0f;
    private Vector3 chargeDirection;

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case MonsterState.Patrol:
                base.FixedUpdate(); // 기본적으로 플레이어를 따라다님

                if (distanceToPlayer <= chargeRange)
                {
                    currentState = MonsterState.ChargeReady;
                    stateTimer = 0.5f; // 돌진 전 준비 시간
                }
                break;

            case MonsterState.ChargeReady:
                // 이 상태에서는 아무것도 하지 않음 (멈춤)
                stateTimer -= Time.fixedDeltaTime;
                if (stateTimer <= 0)
                {
                    currentState = MonsterState.Charging;
                    stateTimer = chargeDuration;
                    chargeDirection = (playerTransform.position - transform.position).normalized;
                }
                break;

            case MonsterState.Charging:
                rb.MovePosition((Vector2)transform.position + (Vector2)chargeDirection * chargeSpeed * Time.fixedDeltaTime);
                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0)
                {
                    // 돌진이 끝나면 Cooldown 상태로 전환
                    currentState = MonsterState.Cooldown;
                    stateTimer = chargeCooldown;
                }
                break;

            case MonsterState.Cooldown:
                base.FixedUpdate(); // 쿨다운 동안에도 플레이어를 따라다님
                stateTimer -= Time.fixedDeltaTime;

                if (stateTimer <= 0)
                {
                    // 쿨다운이 끝나면 다시 Patrol 상태로 돌아감
                    currentState = MonsterState.Patrol;
                }
                break;
        }
    }
}