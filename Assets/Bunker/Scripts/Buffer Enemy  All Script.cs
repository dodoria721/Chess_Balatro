using UnityEngine;
using System.Collections.Generic;

// MonsterMove 클래스를 상속받아 기본 이동 기능을 활용합니다.
public class MonsterSupport : MonsterMove
{
    [Header("버프 효과")]
    public float buffRange;
    public float speedBuffValue;
    public float healthBuffValue;

    private Dictionary<MonsterMove, (float originalSpeed, float originalHealth)> buffedMonsters =
        new Dictionary<MonsterMove, (float, float)>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            MonsterMove monster = other.GetComponent<MonsterMove>();

            if (monster != null && !buffedMonsters.ContainsKey(monster))
            {
                float originalSpeed = monster.EnemymoveSpeed;
                float originalHealth = monster.EnemyHealth;

                monster.EnemyBuff(healthBuffValue, speedBuffValue);

                buffedMonsters.Add(monster, (originalSpeed, originalHealth));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            MonsterMove monster = other.GetComponent<MonsterMove>();

            if (monster != null && buffedMonsters.ContainsKey(monster))
            {
                (float originalSpeed, float originalHealth) = buffedMonsters[monster];

                monster.EnemymoveSpeed = originalSpeed;
                monster.EnemyHealth = originalHealth;

                buffedMonsters.Remove(monster);
            }
        }
    }
}