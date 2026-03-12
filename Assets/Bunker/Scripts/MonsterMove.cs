using UnityEngine;

/*적 플레이어 따라다니게*/
public class MonsterMove : MonoBehaviour
{
    [Header("Monster Info")]
    [SerializeField] public float EnemymoveSpeed;
    [SerializeField] public float EnemyHealth;
    [SerializeField] public float EnemyMaxHealth;
    protected Transform playerTransform;
    protected Rigidbody2D rb;


    // "Player" 태그가 붙은 오브젝트 확인
    protected virtual void Start()
    {
        EnemyHealth = EnemyMaxHealth; // 몬스터 기본 체력
        rb = GetComponent<Rigidbody2D>();

        // 싱글톤 패턴을 통해 찾은 Player 오브젝트 
        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다. 플레이어 오브젝트에 'Player' 태그가 있는지 확인하세요.");
        }
    }

    // 프레임에 따른 호출속도 차이때문에 설정된 호출 시간마다 작동되는 FixedUpdate를 씀
    protected virtual void FixedUpdate()
    {
        if (playerTransform != null)
        {
            // 플레이어로 향하는 방향 계산
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            // 플레이어로 이동
            rb.MovePosition(transform.position + direction * EnemymoveSpeed * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        EnemyHealth -= damage;
        if(EnemyHealth <= 0)
        {
            Die();
        }
    }

    public void EnemyBuff(float EnemyHealthIncrease, float EnemySpeedIncrease)
    {
        EnemymoveSpeed = (1+EnemySpeedIncrease) * EnemymoveSpeed;
        EnemyMaxHealth = (1 + EnemyHealthIncrease) * EnemyMaxHealth;
        EnemyHealth = EnemyHealth * (1+EnemyHealthIncrease);
    }

    public void Die() // 대충 죽는 함수 해놨는데, 보고 바꿔도 됨
    {
        gameObject.SetActive(false);
    }
}