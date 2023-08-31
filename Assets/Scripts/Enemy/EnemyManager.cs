﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("被繼承的關卡程式")]
    public LevelBasic levelBasic;

    [Header("怪物資料")]
    public EnemyData data;

    [Header("怪物轉向器種類")]
    public GameObject monsterRotator;

    [Header("要被資料繼承的血量")]
    public float HP;

    [Header("要被繼承的移動速度")]
    public float speed;

    [Header("當前走到了哪一格")]
    public int wavepointIndex = 0;

    [Header("該怪物的型態")]
    public EnemyType enemyType;
    
    [Header("該怪物種類")]
    public MonsterType monsterType;

    public Transform nextWaypoint;
    private Animator enemyAnimator;
    public int hitcount = 0;
    public int routePlanIndex;
    public int poisonCount = 5; //中毒扣血次數
    private float attackTime = 1; //怪物攻擊的時間
    private float poisonTime = 1; //中毒扣血時間（每1秒扣1次，共扣5次）
    private float changeTime = 0.2f;
    public float frozenTime = 0;
    private float originalSpeed;
    public bool isPoisoning; //是否中毒
    private bool isColorChange = false;
    private bool isSetOriginalColor = false;
    private readonly string bulletLayer = "Bullet";
    private readonly string breakableWallLayer = "Breakable Wall";
    private Color originalColor;

    public void Awake()
    {
        levelBasic = FindObjectOfType<LevelBasic>();
        HP = data.HP;
        speed = data.speed;
        originalSpeed = data.speed;
        originalColor = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    private void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    public void Update()
    {
        #region 外掛
        if (Input.GetKeyDown(KeyCode.X))
        {
            HP = 0;
        }
        #endregion

        if (gameObject.transform.localPosition != Vector3.zero) gameObject.transform.localPosition = Vector3.zero;

        Vector3 dir = nextWaypoint.position - transform.position;
        transform.parent.Translate(speed * Time.deltaTime * dir.normalized, Space.World); //移動要改速度改speed即可

        if (Vector3.Distance(transform.parent.position, nextWaypoint.position) <= 0.002f) GetNextWaypoint(); //換下一個路徑點

        if (HP <= 0) Dead();

        //敵人中毒
        if (isPoisoning)
        {
            poisonTime -= Time.deltaTime;
            ChangeEnemyColor(gameObject, new Color(0.6f, 0.2f, 0.6f, 1f));

            if (poisonTime <= 0)
            {
                HP -= 5;
                poisonCount -= 1;
                poisonTime = 1;
            }

            if (poisonCount <= 0)
            {
                ChangeEnemyColor(gameObject, originalColor);
                isPoisoning = false;
                isSetOriginalColor = false;
                poisonTime = 1;
            }
        }

        if (frozenTime > 0)
        {
            frozenTime -= Time.deltaTime;
            ChangeEnemyColor(gameObject, new Color(0.55f, 0.75f, 1f, 1f));

            if (frozenTime <= 0) 
            {
                speed = originalSpeed;
                ChangeEnemyColor(gameObject, originalColor);
            }
        }

        if (isColorChange) //顏色改變（敵人受傷）
        {
            changeTime -= Time.deltaTime;

            if (changeTime <= 0)
            {
                ChangeEnemyColor(gameObject, originalColor);
                isSetOriginalColor = false;
                changeTime = 0.2f;
                isColorChange = false;
            }
        }

        //轉向判斷
        if (gameObject.transform.parent.position.x - nextWaypoint.transform.position.x > 0.002)
        {
            gameObject.transform.parent.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        else if (gameObject.transform.parent.position.z - nextWaypoint.transform.position.z < -0.002)
        {
            gameObject.transform.parent.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        else if (gameObject.transform.parent.position.x - nextWaypoint.transform.position.x < -0.002)
        {
            gameObject.transform.parent.rotation = Quaternion.Euler(0f, -90f, 0f);
        }

        else if (gameObject.transform.parent.position.z - nextWaypoint.transform.position.z > 0.002)
        {
            gameObject.transform.parent.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public void GetNextWaypoint()
    {
        switch (routePlanIndex)
        {
            case 0:
                if (wavepointIndex >= RoutePlanA.pointA.Length - 1)
                {
                    if (monsterType == MonsterType.Boss)
                    {
                        levelBasic.coreHP -= 10;
                        levelBasic.UpdateCoreHP();
                        Destroy(transform.parent.gameObject);
                    }

                    else
                    {
                        levelBasic.coreHP--;
                        levelBasic.UpdateCoreHP();
                        Destroy(transform.parent.gameObject);
                    }

                    return;
                }

                wavepointIndex++;
                nextWaypoint = RoutePlanA.pointA[wavepointIndex];
                break;
            
            case 1:
                if (wavepointIndex >= RoutePlanB.pointB.Length - 1)
                {
                    if (monsterType == MonsterType.Boss)
                    {
                        levelBasic.coreHP -= 10;
                        levelBasic.UpdateCoreHP();
                        Destroy(transform.parent.gameObject);
                    }

                    else
                    {
                        levelBasic.coreHP--;
                        levelBasic.UpdateCoreHP();
                        Destroy(transform.parent.gameObject);
                    }

                    return;
                }

                wavepointIndex++;
                nextWaypoint = RoutePlanB.pointB[wavepointIndex];
                break;

            case 2:
                if (wavepointIndex >= RoutePlanC.pointC.Length - 1)
                {
                    if (monsterType == MonsterType.Boss)
                    {
                        levelBasic.coreHP -= 10;
                        levelBasic.UpdateCoreHP();
                        Destroy(transform.parent.gameObject);
                    }

                    else
                    {
                        levelBasic.coreHP--;
                        levelBasic.UpdateCoreHP();
                        Destroy(transform.parent.gameObject);
                    }

                    return;
                }

                wavepointIndex++;
                nextWaypoint = RoutePlanC.pointC[wavepointIndex];
                break;
        }
    }

    public void Dead()
    {
        if (monsterType == MonsterType.Minion)
        {
            levelBasic.coin += 10;
            levelBasic.UpdateCoinWallet();
            Destroy(transform.parent.gameObject);
        }

        else if (monsterType == MonsterType.Elite)
        {
            levelBasic.coin += 20;
            levelBasic.UpdateCoinWallet();
            Destroy(transform.parent.gameObject);
        }

        else if (monsterType == MonsterType.Supreme)
        {
            levelBasic.coin += 100;
            levelBasic.UpdateCoinWallet();
            Destroy(transform.parent.gameObject);
        }

        else if (monsterType == MonsterType.Boss)
        {
            levelBasic.winPanel.SetActive(true);

            if (levelBasic.countdownText.gameObject.activeSelf)
                levelBasic.countdownText.gameObject.SetActive(false);
            Time.timeScale = 0f;
            Destroy(transform.parent.gameObject);
        }
    }

    public void OnCollisionStay(Collision other) //碰撞
    {
        if (other.gameObject.layer != LayerMask.NameToLayer(bulletLayer))
        {
            enemyAnimator.SetBool("Attack", true);
        }

        if (isSetOriginalColor == false)
        {
            originalColor = gameObject.GetComponent<MeshRenderer>().material.color;
            isSetOriginalColor = true;
        }

        attackTime -= Time.deltaTime;

        if (attackTime <= 0 && other.gameObject.layer == LayerMask.NameToLayer(breakableWallLayer))
        {
            switch (other.gameObject.tag)
            {
                case "Burn":
                    other.gameObject.GetComponent<WallManager>().HP -= 1;
                    HP -= 10;
                    originalColor = gameObject.GetComponent<MeshRenderer>().material.color;
                    ChangeEnemyColor(gameObject, new Color(1, 0.3f, 0.3f, 1));
                    isColorChange = true;
                    attackTime = 1;
                    break;

                case "Poison":
                    isPoisoning = true;
                    poisonCount = 5;
                    other.gameObject.GetComponent<WallManager>().HP -= 1;
                    break;

                case "Normal":
                    other.gameObject.GetComponent<WallManager>().HP -= 1;
                    break;
            }

            attackTime = 1;
            enemyAnimator.SetBool("Attack", false);
        }

        if ((other.gameObject.CompareTag("Burn") || other.gameObject.CompareTag("Poison") || other.gameObject.CompareTag("Normal")) && other.gameObject.GetComponent<WallManager>().HP <= 0)
        {
            attackTime = 1;
            enemyAnimator.SetBool("Attack", false);
        }
    }

    public void OnCollisionExit(Collision other)
    {
        attackTime = 1;
        enemyAnimator.SetBool("Attack", false);
    }

    public void ChangeEnemyColor(GameObject enemy, Color color)
    {
        switch (enemy.name)
        {
            case "NormalMinion(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                break;

            case "SpeedMinion(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                
                for (int i = 1; i <= 2; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "DefenseMinion(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;

                for (int i = 1; i <= 3; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "NormalElite(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                break;

            case "SpeedElite(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                
                for (int i = 1; i <= 2; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "DefenseElite(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                
                for (int i = 1; i <= 3; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "AttackSupreme(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                
                for (int i = 1; i <= 8; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "SpeedSupreme(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                
                for (int i = 1; i <= 3; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "DefenseSupreme(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                
                for (int i = 1; i <= 6; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "AttackBoss(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                
                for (int i = 1; i <= 8; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "SpeedBoss(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                
                for (int i = 1; i <= 3; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;

            case "DefenseBoss(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;

                for (int i = 1; i <= 6; i++)
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                
                break;
        }
    }
}