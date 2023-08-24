using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
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

    private Transform nextWaypoint;
    private Animator enemyAnimator;
    private int poisonCount = 5; // 中毒扣血次數
    private float attackTime = 1; // 怪物攻擊的時間
    private float poisonTime = 1; // 中毒扣血時間（每1秒扣1次，共扣5次）
    private float changeTime = 0.2f;
    private bool isPoisoning; // 是否中毒
    private bool isColorChange = false;
    private bool isSetOriginalColor = false;
    private readonly string bulletLayer = "Bullet";
    private readonly string breakableWallLayer = "Breakable Wall";
    private Color originalColor;

    public void Awake()
    {
        levelBasic = FindObjectOfType<LevelBasic>();
        HP = data.HP;
        speed = data.Speed;
    }

    private void Start()
    {
        nextWaypoint = RoutePlanA.pointA[0]; // 目標 = 路徑點
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

        Vector3 dir = nextWaypoint.position - transform.position;
        transform.parent.Translate(speed * Time.deltaTime * dir.normalized, Space.World); // 移動 要改速度改speed即可

        if (Vector3.Distance(transform.parent.position, nextWaypoint.position) <= 0.002f) // 換下一個路徑點
        {
            GetNextWaypoint();
        }

        if (HP <= 0)
        {
            Dead();
        }

        if (isPoisoning) // 敵人中毒
        {
            poisonTime -= Time.deltaTime;
            ChangeEnemyColor(gameObject, new Color(0.6f, 0.2f, 0.6f, 1));
            // Debug.Log("time" + PoisonTime);
            if (poisonTime <= 0)
            {
                HP -= 5;
                //Debug.Log("Current HP of " + gameObject.ToString() + " is " + HP);
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

        if (isColorChange) // 顏色改變（敵人受傷）
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

        // 轉向判斷
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
        if (wavepointIndex >= RoutePlanA.pointA.Length - 1) // 到達核心就消失（目前）
        {
            if (monsterType == MonsterType.Boss)
            {
                levelBasic.CoreHP -= 10;
                levelBasic.UpdateCoreHP();
                Destroy(transform.parent.gameObject);
            }

            else
            {
                levelBasic.CoreHP--;
                levelBasic.UpdateCoreHP();
                Destroy(transform.parent.gameObject);
            }
            return;
        }

        wavepointIndex++;
        nextWaypoint = RoutePlanA.pointA[wavepointIndex];
    }

    public void Dead()
    {
        if (monsterType == MonsterType.Minion)
        {
            levelBasic.Coin += 10;
            levelBasic.UpdateCoinWallet();
            Destroy(transform.parent.gameObject);
        }

        else if (monsterType == MonsterType.Elite)
        {
            levelBasic.Coin += 20;
            levelBasic.UpdateCoinWallet();
            Destroy(transform.parent.gameObject);
        }

        else if (monsterType == MonsterType.Supreme)
        {
            levelBasic.Coin += 100;
            levelBasic.UpdateCoinWallet();
            Destroy(transform.parent.gameObject);
        }

        else if (monsterType == MonsterType.Boss)
        {
            print("我打敗Boss了，我可以通關了");
            levelBasic.winPanel.SetActive(true);
            Time.timeScale = 0f;
            Destroy(transform.parent.gameObject);
        }
    }

    public void OnCollisionEnter(Collision other) // 碰撞
    {
        //Debug.Log("觸發碰撞事件");
    }

    public void OnCollisionStay(Collision other) // 碰撞
    {
        //Debug.Log("持續碰撞" + other.gameObject);
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer(bullet))
    //    {
    //        HP -= 5;
    //        Debug.Log("擊中敵人！ Hp: " + HP);
    //    }
    //}

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
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "DefenseMinion(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 3; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "NormalElite(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                break;

            case "SpeedElite(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 2; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "DefenseElite(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 3; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "AttackSupreme(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 8; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "SpeedSupreme(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 3; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "DefenseSupreme(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 6; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "AttackBoss(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 8; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "SpeedBoss(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 3; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;

            case "DefenseBoss(Clone)":
                enemy.GetComponent<MeshRenderer>().material.color = color;
                for (int i = 1; i <= 6; i++)
                {
                    enemy.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = color;
                }
                break;
        }
    }
}
