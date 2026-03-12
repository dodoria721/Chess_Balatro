using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("# Game Object")]
    public Player player;
    public PoolManager pool;
    public LevelUp uiLevelUp;
    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    // public float maxGameTime = ?;
    // 게임 최종 시간
    [Header("# Player Info")]
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 10, 30, 60, 100, 150, 210, 280, 360, 450, 600 };

    // 싱글톤 패턴
    public static GameManager Instance { get; private set; }

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
    }

    public void GameStart()
    {
        health = maxHealth;
        uiLevelUp.Select(0);
        uiLevelUp.Select(1); 
        uiLevelUp.Select(2); // 임시 스크립트 (캐릭터 기본 무기)
        isLive = true;
    }

    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;
        /*
        if (gameTime > maxGameTime) {
            gameTime = maxGameTime;
        }
        */
    }

    public void GetExp()
    {
        exp++;

        if (exp >= nextExp[Mathf.Min(level, nextExp.Length-1)]) // 레벨업 요구 경험치를 일정 수준 이상으로 늘리지 않고 고정하는 건데 아직 레벨업 요구 경험치 관련해서 확정난게 없음
        {
            level++;
            exp = exp - nextExp[level];
            uiLevelUp.Show();
        }
    }

    //시간 멈추는 건데 일단은 레벨업 할 떄 만 멈춤. 멈출떄 isLive변수를 Update에서 사용해야됨
    /*
    대충 이거 가져다 붙이면 멈춤
    
            if (!GameManager.instance.isLive)
            return;
    
     */
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }

}
