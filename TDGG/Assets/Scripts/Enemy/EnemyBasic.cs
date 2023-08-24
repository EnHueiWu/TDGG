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
    private bool isPoisoned; // 是否中毒
    private bool isColorChanged = false;
    private readonly string bullet = "Bullet";
    private readonly string breakableWall = "Breakable Wall";
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

        if (isPoisoned) // 敵人中毒
        {
            poisonTime -= Time.deltaTime;
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0.2f, 0.6f, 1);
            // Debug.Log("time" + PoisonTime);
            if (poisonTime <= 0)
            {
                HP -= 5;
                Debug.Log(HP);
                poisonCount -= 1;
                poisonTime = 1;
            }

            if (poisonCount <= 0)
            {
                gameObject.GetComponent<MeshRenderer>().material.color = originalColor;
                isPoisoned = false;
                poisonTime = 1;
            }
        }

        if (isColorChanged) // 顏色改變（敵人受傷）
        {
            changeTime -= Time.deltaTime;
            if (changeTime <= 0)
            {
                gameObject.GetComponent<MeshRenderer>().material.color = originalColor;
                changeTime = 0.2f;
                isColorChanged = false;
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
        Debug.Log("觸發碰撞事件");
    }

    public void OnCollisionStay(Collision other) // 碰撞
    {
        Debug.Log("持續碰撞" + other.gameObject);
        if (other.gameObject.layer != LayerMask.NameToLayer(bullet))
        {
            enemyAnimator.SetBool("Attack", true);
        }
        originalColor = gameObject.GetComponent<MeshRenderer>().material.color;
        attackTime -= Time.deltaTime;

        if (attackTime <= 0 && other.gameObject.layer == LayerMask.NameToLayer(breakableWall))
        {
            switch (other.gameObject.tag)
            {
                case "Burn":
                    other.gameObject.GetComponent<WallManager>().HP -= 1;
                    HP -= 10;
                    gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0.3f, 0.3f, 1);
                    isColorChanged = true;
                    attackTime = 1;
                    break;
                case "Poison":
                    isPoisoned = true;
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

        if ((other.gameObject.tag == "Burn" || other.gameObject.tag == "Poison" || other.gameObject.tag == "Normal") && other.gameObject.GetComponent<WallManager>().HP <= 0)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(bullet))
        {
            HP -= 5;
            Debug.Log("擊中敵人！ Hp: " + HP);
        }
    }
}
