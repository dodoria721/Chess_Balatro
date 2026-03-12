using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Wepon Info")]
    public int id;               // 무기 고유 id
    private int prefabId;        // poolmanager에 설정된 id(인덱스 값)
    private float damage;        // 데미지
    private float speed;         // 무기 자체의 이동 속도
    private int count;           // 무기의 수
    private int per;             // 관통
    public float rateOfFire;     // 발사 속도
    public float rotationSpeed;  // 회전 속도

    float timer;
    Player player;
    Vector3 dir;

    // 각 무기의 상태를 초기화 하는 함수
    public void Init(ItemData data)
    {
        // basic 세팅
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // preperty 세팅
        id = data.itemId;
        damage = data.baseDamage;
        speed = data.baseSpeed;
        count = data.baseCount;
        per = data.basePer;
        rateOfFire = data.baserateOfFire;
        rotationSpeed = data.baserotationSpeed;

        for (int index=0; index < GameManager.Instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.Instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }
        switch (id)
        {
            case 0:
                Batch();
                break;
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 주기적으로 총알을 발사하거나, 플레이어 주변 무기를 돌게 하거나
    void Update()
    {
        //Debug.Log($"id: {id}");
        if (!GameManager.Instance.isLive)
            return;

        switch (id)
        {
            case 0: // 플레이어 주변 돌아가는 무기
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
                break;
            case 1:
                timer += Time.deltaTime;

                if (timer > rateOfFire)
                {
                    timer = 0f;
                    Fire();
                }
                break;
            case 2:
                timer += Time.deltaTime;

                if(timer > rateOfFire)
                {
                    timer = 0f;
                    ProjectileFire();
                }
                break;
        }
    }

    public void LevelUp(WeaponLevelUpData data)
    {
        // 제공된 데이터가 있는 경우에만 업데이트
        if (data.nextDamage != 0) this.damage = data.nextDamage;
        if (data.nextCount != 0) this.count += data.nextCount;
        if (data.nextPer != 0) this.per += data.nextPer;
        if (data.nextSpeed != 0) this.speed += data.nextSpeed;
        if (data.nextRateOfFire != 0) this.rateOfFire += data.nextRateOfFire;
        if (data.nextRotationSpeed != 0) this.rotationSpeed += data.nextRotationSpeed;

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    private void Awake()
    {
        player = GameManager.Instance.player;
    }


    // 플레이어 주변 돌아다니는 무기 설정
    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            GameObject newMeleeObject = GameManager.Instance.pool.Get(prefabId);
            IMelee orbitalBlade = newMeleeObject.GetComponent<IMelee>();

            if (orbitalBlade == null)
            {
                Debug.LogError("오브젝트 풀에서 가져온 프리팹에 OrbitalBlade 컴포넌트가 없습니다.");
                continue;
            }

            // OrbitalBlade의 부모를 Weapon 오브젝트로 설정
            newMeleeObject.transform.SetParent(this.transform);

            float angle = 360f / count * index;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 localPos = rotation * Vector3.up * 1.5f;
            newMeleeObject.transform.localPosition = localPos;

            // 추후 몬스터와 충돌시를 위한 스크립트 호출
            //orbitalBlade.Init(this.damage);
        }
    }

    // 원거리 무기 설정
    void Fire()
    {
        if (!Scanner.Instance.nearestTarget)
        {
            return;
        }

        Vector3 targetPos = Scanner.Instance.nearestTarget.position;
        dir = (targetPos - transform.position).normalized;

        // 부채꼴 모양 발사를 위해 각도 계산
        float fanAngle = 45f; // 부채꼴 각도
        float startAngle = -fanAngle / 2f;
        float angleStep = (count > 1) ? fanAngle / (count - 1) : 0;

        for (int i = 0; i < count; i++)
        {
            GameObject newRangeObject = GameManager.Instance.pool.Get(prefabId);
            IRange range = newRangeObject.GetComponent<IRange>();
       
            if (range == null)
            {
                Debug.LogError("오브젝트 풀에서 가져온 프리팹에 IRange 컴포넌트가 없습니다.");
                continue;
            }

            Transform rangeTransform = newRangeObject.transform;
            rangeTransform.position = transform.position;

            // 회전 각도 계산
            float currentAngle = startAngle + i * angleStep;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, currentAngle) * Quaternion.FromToRotation(Vector3.up, dir);
            rangeTransform.rotation = bulletRotation;

            // 총알 방향을 회전된 방향으로 설정
            Vector3 rotatedDir = bulletRotation * Vector3.up;
            range.Init(this.speed, this.damage, this.per, rotatedDir);
        }
    }

    // 투사체 무기 발사
    void ProjectileFire()
    {
        // 각 투사체가 날아갈 총 비행 시간과 궤적의 높이
        float grenadeFlightTime = 1.5f;
        float grenadeArcHeight = 2f;
        float grenadeRadius = 3f; // 원형 발사의 반지름

        for (int i = 0; i < count; i++)
        {
            // 각 투사체의 각도 계산 (360도를 무기의 수로 나눔)
            float angle = 360f / count * i;

            // 각도를 이용하여 발사 방향 벡터 계산
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 fireDir = rotation * Vector3.up;

            // 목표 지점 계산: 플레이어 위치 + (방향 벡터 * 반지름)
            Vector3 targetPos = player.transform.position + fireDir.normalized * grenadeRadius;

            // 오브젝트 풀에서 투사체 가져오기
            GameObject newProjectileObject = GameManager.Instance.pool.Get(prefabId);
            IProjectile grenade = newProjectileObject.GetComponent<IProjectile>();
            
            if (grenade == null)
            {
                Debug.LogError("오브젝트 풀에서 가져온 프리팹에 Grenade 컴포넌트가 없습니다.");
                continue;
            }

            // 투사체의 위치를 플레이어 위치로 설정
            newProjectileObject.transform.position = player.transform.position;

            // Grenade.cs의 Init 함수를 호출하여 포물선 궤적 초기화
            grenade.Init(player.transform.position, targetPos, grenadeFlightTime, grenadeArcHeight);
        }
    }
}