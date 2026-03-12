using UnityEngine;

public class OrbitalBlade : MonoBehaviour, IMelee
{
    private float Damage;
    private Rigidbody2D rigid;

    public void Init(float damage)
    {
        this.Damage = damage;
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
}