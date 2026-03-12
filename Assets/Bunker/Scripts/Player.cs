using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    // 초당 움직이는 유닛수
    public float moveSpeed;
    public float baseSpeed;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    /*메모리 절약을 위한 싱글톤 패턴
     * 기존의 코드는 MonsterMove.cs 에서 플레이어를 찾을때 Player 태그가 있는 오브젝트를 찾도록 구현되었다.
     * 다만 이는 몬스터의 수가 많아 질수록 각각의 emney 오브젝트가 생성될때마다, Plyaer 태그를 찾아 메모리 소모가 크다.
     * 이를 위해 싱글톤 패턴을 이용, 하나의 Player 인스턴스만 사용하도록 구현
     */
    public static Player Instance { get; private set; }


    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        rb = GetComponent<Rigidbody2D>();
        baseSpeed = moveSpeed;
    }



    private void FixedUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 moveVelocity = moveInput.normalized * moveSpeed;
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.Instance.isLive)
            return;

        GameManager.Instance.health -= Time.deltaTime * 10; 

        if (GameManager.Instance.health < 0)
        {

        }
    }
}
