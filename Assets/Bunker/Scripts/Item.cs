using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()  // 레벨업할때 화며에 Item 의 UI 가 활성화 될 시
    {
        icon = GetComponentsInChildren<Image>()[0]; // 해당 스크립트가 설정되있는 오브제트의 자식중 이미지 컴포넌트를 찾는데 그중 첫번째 위치한거
        icon.sprite = data.itemIcon;  //화면에 표시될 아이콘

        Text[] texts = GetComponentsInChildren<Text>(); // 자식들중 Text 컴포넌트를 찾음
        textLevel = texts[0]; // Text 컴포넌트 중에 계층 구조상 첫번째 위치 TextLevel
        textName = texts[1];  // Text 컴포넌트 중에 계층 구조상 두번째 위치 TextName
        textDesc = texts[2];  // Text 컴포넌트 중에 계층 구조상 세번째 위치 TextDesc
        textName.text = data.itemName;  // 화면에 표시될 아이템 text
    }

    void OnEnable() // 레벨업시 무기가 어떻게 변할지를 알려주는 설명을 설정하는 부분
    {
        textLevel.text = "Lv. " + level;

        switch(data.itemType)  // 해당 무기의 desc에 설정되있는 플레이스 홀더{0},{1}을 설정하는 부분
        {
            case ItemData.ItemType.Melee: 
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.pers[level]);
                break;
            case ItemData.ItemType.Projectile:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemData.ItemType.GearItem0:
                textDesc.text = string.Format(data.itemDesc, data.rateOfFires[level] * 100);
                break;
            case ItemData.ItemType.GearItem1:
                textDesc.text = string.Format(data.itemDesc, data.rotationSpeeds[level] * 100);
                break;
        }
        
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee: // 근접무기
                if(level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    WeaponLevelUpData levelUpData = new WeaponLevelUpData();

                    levelUpData.nextDamage = data.baseDamage + data.baseDamage * data.damages[level];
                    levelUpData.nextCount = data.counts[level];

                    weapon.LevelUp(levelUpData);
                }
                    break;

            case ItemData.ItemType.Range: // 원거리 무기
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    WeaponLevelUpData levelUpData = new WeaponLevelUpData();

                    levelUpData.nextDamage = data.baseDamage + data.baseDamage * data.damages[level];
                    levelUpData.nextCount = data.counts[level];
                    levelUpData.nextPer = data.pers[level];
                    levelUpData.nextRateOfFire = data.rateOfFires[level];

                    weapon.LevelUp(levelUpData);
                }
                    break;

            case ItemData.ItemType.Projectile: // 투사체 무기
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    WeaponLevelUpData levelUpData = new WeaponLevelUpData();

                    levelUpData.nextDamage = data.baseDamage + data.baseDamage * data.damages[level];
                    levelUpData.nextCount = data.counts[level];
                    levelUpData.nextRateOfFire = data.rateOfFires[level];

                    weapon.LevelUp(levelUpData);
                }
                break;

            case ItemData.ItemType.GearItem0:
            case ItemData.ItemType.GearItem1:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.rateOfFires[level];
                    gear.LevelUp(nextRate);
                }
                    break;
        }

        level++;

        if (level == data.levels.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
