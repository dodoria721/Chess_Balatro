using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type; 
    public float rate;  // 해당 장비가 제공하는 능력치 증가율

    public void Init(ItemData data)
    {
        // basic set
        name = "Gear" + data.itemId;
        transform.parent = GameManager.Instance.player.transform;
        transform.localPosition = Vector3.zero;

        // property set
        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.GearItem0: // 연사속도 증가
                RateUp();
                break;
            case ItemData.ItemType.GearItem1: // 플레이어 이동속도 증가
                SpeedUp();
                break;
        }
    }

    void RateUp() // 연사속도 증가 함수
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();  // 플레이어 자식 오브젝트에 설정된 무기를 다 가져옴

        foreach(Weapon weapon in weapons)
        {
            switch(weapon.id) // 무기 id에 따라 나눔
            {
                case 1: //원거리 무기
                case 2: // 투사체 무기
                    weapon.rateOfFire = weapon.rateOfFire * (1 + rate);
                    break;
            }
        }
    }

    void SpeedUp()
    {
        float baseSpeed = GameManager.Instance.player.baseSpeed;
        GameManager.Instance.player.moveSpeed = baseSpeed + (baseSpeed * rate);
    }
}
