using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Projectile, GearItem0, GearItem1 } //gearitem은 패시브 스킬

    [Header("# Main Info")]
    public ItemType itemType; //아이템 타입
    public int itemId;        // 아이템 코드
    public string itemName;   // 아이템 이름
    [TextArea]
    public string itemDesc;   // 아이템 설명
    public Sprite itemIcon;   // 아이템 아이콘

    [Header("# Level Data")]
    public int baseLevel;
    public float baseDamage;
    public float baseSpeed;
    public int baseCount;  //총알의 수
    public int basePer;    // 관통력
    public float baserateOfFire;  // 발사 속도
    public float baserotationSpeed; // 회전 속도

    public int[] levels;
    public float[] damages;
    public float[] speeds;
    public int[] counts;
    public int[] pers;
    public float[] rateOfFires;
    public float[] rotationSpeeds;

    [Header("# Weapon")]
    public GameObject projectile;

}
