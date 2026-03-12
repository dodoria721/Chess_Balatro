using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour, IRange
{
    [Header("내부 컴포넌트")]
    private float damage;
    private int per;
    private Rigidbody2D rigid;

    // 충돌했던 적들을 추적하여 중복 충돌을 방지하는 리스트
    private List<Collider2D> collidedEnemies = new List<Collider2D>();

    private string ownerTag;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float speed, float damage, int per, Vector3 dir, string ownerTag = null)
    {
        this.per = per;
        this.ownerTag = ownerTag;

        // 총알이 오브젝트 풀에서 재활용될 때마다 충돌 기록 초기화
        collidedEnemies.Clear();

        if (per > -1)
        {
            // dir 방향으로, bulletSpeed 속도로 이동
            rigid.linearVelocity = dir * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // A. 충돌 대상이 총알을 쏜 발사체(자기 자신)이거나,
        // 플레이어의 총알이 이미 충돌했던 적일 경우 함수 종료
        if (ownerTag != null && collision.CompareTag(ownerTag))
        {
            return;
        }

        if (collidedEnemies.Contains(collision))
        {
            return;
        }

        // B. 총알이 적이 발사한 것일 경우
        if (ownerTag == "Enemy")
        {
            // 충돌한 대상이 플레이어인지 확인
            if (collision.CompareTag("Player"))
            {
                // 여기에 플레이어에게 데미지를 주는 코드 추가
                // e.g., collision.GetComponent<Player>().TakeDamage(damage);

                // 관통 없이 바로 사라짐
                rigid.linearVelocity = Vector2.zero;
                gameObject.SetActive(false);
            }
        }
        // C. 총알이 플레이어가 발사한 것일 경우
        else if (ownerTag == null || ownerTag == "Player")
        {
            // 충돌한 대상이 적인지 확인
            if (collision.CompareTag("Enemy"))
            {
                // 충돌한 적을 리스트에 추가 (관통 기능을 위해)
                collidedEnemies.Add(collision);

                // 여기에 적에게 데미지를 주는 코드 추가
                // e.g., collision.GetComponent<Enemy>().TakeDamage(damage);

                // per가 -1 (무제한 관통)이 아닐 때만 횟수 감소
                if (per > -1)
                {
                    per--;
                }

                // 관통 횟수가 모두 소진되면 총알 비활성화
                if (per < 0)
                {
                    rigid.linearVelocity = Vector2.zero;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}