using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class BossSpawnData
{
    public int waveNumber; // 特定波數
    public GameObject bossPrefab; // 小Boss預製件
}

public class LevelBasic : MonoBehaviour
{
    public static LevelBasic Instance { get; private set; }
    public BossSpawnData[] bossSpawnData;
    public bool spawnedBoss;

    #region 抓取檔案以及要被繼承的資料
    [Header("關卡資料結構")]
    public LevelData levelData;

    [Header("要被資料繼承的怪物總數量")]//測試完畢後可以將要繼承的東西全部改成私有private
    private int EnemyQuantity;

    [Header("要繼承出現正常型態怪物數量")]
    private int NormalEnemy;

    [Header("要繼承出現速度型態怪物數量")]
    private int SpeedEnemy;

    [Header("要繼承出現防禦型態怪物數量")]
    private int DefendEnemy;

    [Header("要繼承出現菁英正常型態怪物數量")]
    private int NormalElite;

    [Header("該關卡出現菁英速度型態怪物數量")]
    private int SpeedElite;

    [Header("該關卡出現菁英防禦型態怪物數量")]
    private int DefendElite;

    [Header("該關卡出現的Boss")]
    public GameObject Boss;

    [Header("關卡波數")]
    public int wave;

    [Header("波數怪物數量陣列")]
    public int[] EnemyWave;

    [Header("關卡核心血量")]
    public int CoreHP;

    [Header("關卡金幣")]
    public int Coin;
    #endregion

    #region UI物件
    [Header("UI時間倒數計時器")]
    private TextMeshProUGUI countdownText;

    [Header("UI波數說明")]
    private TextMeshProUGUI waveText;
    private TextMeshProUGUI condensedWaveText;

    [Header("UI金幣")]
    private TextMeshProUGUI coinText;

    [Header("UI核心血量")]
    private TextMeshProUGUI HPtext;

    [Header("UI過關視窗")]
    [HideInInspector]
    public GameObject winPanel;
    [Header("UI失敗視窗")]
    private GameObject losePanel;

    [Header("暫停視窗")]
    private GameObject pausePanel;

    [Header("1開始按鈕,2直接下一波按鈕,3暫停按鈕,4暫停介面中關閉暫停按鈕")]
    private Button startButton;
    private Button nextWaveButton;
    private Button pauseButton;
    private Button closeButton;
    private GameObject waveDisplay;

    [Header("抓取重進遊戲的按鈕")]
    private Button[] restartButton;
    private GameObject[] restart;

    [Header("抓取回到首頁的按鈕")]
    private Button[] menuButton;
    private GameObject[] menu;

    [Header("按鈕物件用來隱藏")]
    private GameObject start;
    private GameObject nextWave;
    private GameObject pause;
    #endregion

    #region 怪物預置物，以及設置怪物生成點
    // 0 正常型態怪物
    // 1 速度型態怪物
    // 2 防禦型態怪物
    // 3 菁英正常型態怪物
    // 4 菁英速度型態怪物
    // 5 菁英防禦型態怪物
    //小Boss與Boss另外寫法
    [Header("怪物預置物陣列，腳本內有詳細順序，共需要6格，不包含小Boss")]
    public GameObject[] Monsters = new GameObject[6];

    [Header("怪物生成點，如果該關卡只有1個重生點複製同樣位置，改名就好")]
    private GameObject MonsterSpawnPointA, MonsterSpawnPointB, MonsterSpawnPointC;
    #endregion

    #region 核心、砲台、城牆
    //public GameObject Core;
    #endregion

    float countdownTime = 10f; // 倒數計時的秒數，這裡設定為 10秒
    bool countdownStart; //倒數計時開始布林

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //初始化遊戲時間
        Time.timeScale = 1f;

        #region 該處是繼承資料
        this.EnemyQuantity = levelData.enemyQuantity;
        this.NormalEnemy = levelData.normalEnemy;
        this.SpeedEnemy = levelData.speedEnemy;
        this.DefendEnemy = levelData.defenseEnemy;
        this.NormalElite = levelData.normalElite;
        this.SpeedElite = levelData.speedElite;
        this.DefendElite = levelData.defenseElite;
        this.Boss = levelData.boss;
        this.wave = levelData.wave;
        this.EnemyWave = levelData.enemyWave;
        this.CoreHP = levelData.coreHP;
        this.Coin = levelData.coin;
        #endregion

        #region UI按鈕、介面 靠程式抓取   這個需要全部開啟才能被抓取，如果關閉會抓取不到
        countdownText = GameObject.Find("CountdownText").GetComponent<TextMeshProUGUI>();
        waveText = GameObject.Find("WaveText").GetComponent<TextMeshProUGUI>();
        condensedWaveText = GameObject.Find("CondensedWaveText").GetComponent<TextMeshProUGUI>();
        coinText = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
        HPtext = GameObject.Find("HPText").GetComponent<TextMeshProUGUI>();
        winPanel = GameObject.Find("WinPanel");
        losePanel = GameObject.Find("LosePanel");
        pausePanel = GameObject.Find("PausePanel");
        start = GameObject.Find("Start");
        nextWave = GameObject.Find("NextWave");
        waveDisplay = GameObject.Find("WaveDisplay");
        pause = GameObject.Find("Pause");
        startButton = start.GetComponent<Button>();
        nextWaveButton = nextWave.GetComponent<Button>();
        pauseButton = pause.GetComponent<Button>();
        closeButton = GameObject.Find("Close").GetComponent<Button>();
        restart = GameObject.FindGameObjectsWithTag("Restart");
        restartButton = new Button[restart.Length];
        for (int i = 0; i < restart.Length; i++)
        {
            restartButton[i] = restart[i].GetComponent<Button>();
        }
        menu = GameObject.FindGameObjectsWithTag("Menu");
        menuButton = new Button[menu.Length];
        for (int i = 0; i < menu.Length; i++)
        {
            menuButton[i] = menu[i].GetComponent<Button>();
        }
        #endregion

        #region 陣列抓取敵人數量、設定怪物重生點
        MonsterSpawnPointA = GameObject.Find("SpawnPointA");
        MonsterSpawnPointB = GameObject.Find("SpawnPointB");
        MonsterSpawnPointC = GameObject.Find("SpawnPointC");
        // 先將可用的怪物類型加入到 availableEnemies 中
        remainingEnemies = new int[6];
        remainingEnemies[0] = this.NormalEnemy;
        remainingEnemies[1] = this.SpeedEnemy;
        remainingEnemies[2] = this.DefendEnemy;
        remainingEnemies[3] = this.NormalElite;
        remainingEnemies[4] = this.SpeedElite;
        remainingEnemies[5] = this.DefendElite;
        if (NormalEnemy > 0) availableEnemies.Add(0);
        if (SpeedEnemy > 0) availableEnemies.Add(1);
        if (DefendEnemy > 0) availableEnemies.Add(2);
        if (NormalElite > 0) availableEnemies.Add(3);
        if (SpeedElite > 0) availableEnemies.Add(4);
        if (DefendElite > 0) availableEnemies.Add(5);
        #endregion
    }

    void Start()
    {
        #region UI按鈕、介面抓取後部分隱藏，部分抓取腳本
        winPanel.GetComponent<CanvasGroup>().alpha = 1;
        losePanel.GetComponent<CanvasGroup>().alpha = 1;
        pausePanel.GetComponent<CanvasGroup>().alpha = 1;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        pausePanel.SetActive(false);
        nextWave.SetActive(false);
        startButton.onClick.AddListener(StartGame);
        nextWaveButton.onClick.AddListener(ClickNextWave);
        pauseButton.onClick.AddListener(ClickStopButtom);
        closeButton.onClick.AddListener(ClickStopButtom);
        for (int i = 0; i < restartButton.Length; i++)
        {
            restartButton[i].onClick.AddListener(ResetLevel);
        }
        for (int i = 0; i < menuButton.Length; i++)
        {
            menuButton[i].onClick.AddListener(ToMenu);
        }
        #endregion

        UpdateCoinWallet();
        UpdateCoreHP();
    }

    void Update()
    {
        #region 外掛
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CoreHP = 100;
            UpdateCoreHP();
        }
        #endregion

        #region 波數倒數計時器
        if (countdownStart && !OpenStopPanel)
        {
            countdownText.gameObject.SetActive(true);
            int seconds;
            seconds = Mathf.FloorToInt(countdownTime % 60f);
            countdownTime -= Time.deltaTime;
            // 將秒數轉換為字串形式，並顯示在 UI 上
            countdownText.text = "下一波時間倒數：" + string.Format("{0:00}", seconds);
        }
        else
        {
            countdownText.gameObject.SetActive(false);
        }
        #endregion

        #region 核心血量判斷
        if (CoreHP <= 0)
        {
            losePanel.SetActive(true);
            Time.timeScale = 0;
        }
        #endregion
    }

    [Header("目前第幾波")]
    public int currentWave = 0; // 目前的波數索引
    [Header("每種怪物剩餘的數量")]
    public int[] remainingEnemies;   // 儲存每種怪物剩餘的數量

    [Header("當每種怪物數量>0時該處便會擁有陣列")]
    public List<int> availableEnemies = new List<int>();

    public float[] enemySpawnProbabilities;

    // 呼叫此方法生成下一波怪物
    public IEnumerator SpawnNextWave()
    {
        if (currentWave < wave)
        {
            int totalEnemies = EnemyWave[currentWave];

            // 計算每個敵人類型的生成機率
            enemySpawnProbabilities = new float[remainingEnemies.Length];
            float totalProbability = 0;

            for (int i = 0; i < remainingEnemies.Length; i++)
            {
                totalProbability += remainingEnemies[i];
                enemySpawnProbabilities[i] = totalProbability;
            }

            // 隨機抽取怪物類型並生成
            for (int i = 0; i < totalEnemies; i++)
            {
                if (availableEnemies.Count > 0)
                {
                    Vector3 spawnPosition;
                    int spawnPointIndex = Random.Range(0, 3);

                    switch (spawnPointIndex)
                    {
                        case 0:
                            spawnPosition = MonsterSpawnPointA.transform.position;
                            break;
                        case 1:
                            spawnPosition = MonsterSpawnPointB.transform.position;
                            break;
                        case 2:
                            spawnPosition = MonsterSpawnPointC.transform.position;
                            break;
                        default:
                            spawnPosition = Vector3.zero; // 預設位置
                            break;
                    }

                    // 隨機生成機率值，決定生成哪個怪物
                    float randomProbability = Random.Range(0, totalProbability);
                    int enemyType = 0;

                    for (int j = 0; j < enemySpawnProbabilities.Length; j++)
                    {
                        if (randomProbability < enemySpawnProbabilities[j])
                        {
                            enemyType = j;
                            break;
                        }
                    }

                    switch (enemyType)
                    {
                        case 0:
                            // 生成正常型態怪物
                            InstantiateEnemy(Monsters[0], spawnPosition);
                            // 減去剩餘數量
                            remainingEnemies[0]--;
                            break;
                        case 1:
                            // 生成速度型態怪物
                            InstantiateEnemy(Monsters[1], spawnPosition);
                            // 減去剩餘數量
                            remainingEnemies[1]--;
                            break;
                        case 2:
                            // 生成防禦型態怪物
                            InstantiateEnemy(Monsters[2], spawnPosition);
                            // 減去剩餘數量
                            remainingEnemies[2]--;
                            break;
                        case 3:
                            // 生成正常型態的菁英怪物
                            InstantiateEnemy(Monsters[3], spawnPosition);
                            // 減去剩餘數量
                            remainingEnemies[3]--;
                            break;
                        case 4:
                            // 生成速度型態的菁英怪物
                            InstantiateEnemy(Monsters[4], spawnPosition);
                            // 減去剩餘數量
                            remainingEnemies[4]--;
                            break;
                        case 5:
                            // 生成防禦型態的菁英怪物
                            InstantiateEnemy(Monsters[5], spawnPosition);
                            // 減去剩餘數量
                            remainingEnemies[5]--;
                            break;
                    }


                    // 更新機率總和
                    totalProbability = 0;
                    for (int j = 0; j < remainingEnemies.Length; j++)
                    {
                        totalProbability += remainingEnemies[j];
                        enemySpawnProbabilities[j] = totalProbability;
                    }

                    if (ShouldSpawnBoss(currentWave) && !spawnedBoss) // 檢查是否需要生成小Boss
                    {
                        Debug.LogWarning("我有生成小Boss喔");
                        GameObject bossPrefab = GetBossPrefabForWave(currentWave);
                        InstantiateEnemy(bossPrefab, spawnPosition);
                        spawnedBoss = true;
                    }

                    // 如果該類型的怪物剩餘數量為0，從可用怪物列表中移除
                    if (remainingEnemies[enemyType] == 0)
                    {
                        availableEnemies.Remove(enemyType);
                    }
                    yield return new WaitForSeconds(0.5f);
                }

                else
                {
                    print("我應該跑不到這裡，我在這除錯用");
                }
            }

            if (availableEnemies.Count != 0)
            {
                print("怪物還有你還可以在執行");
                WaveCompleted();
                currentWave++;
                // 在生成完怪物後，將 spawnedBoss 設為 false
                spawnedBoss = false;
                //UpdateWave();
            }

            else
            {
                // 隨機選擇一個生成位置，可以是兩個點位中的一個
                Vector3 spawnPosition = Random.Range(0, 2) == 0 ? MonsterSpawnPointA.transform.position : MonsterSpawnPointB.transform.position;
                print("我要生成Boss");
                InstantiateEnemy(Boss, spawnPosition);
                //print(spawnPosition);
            }
        }
    }

    Coroutine NextCoroutine;
    Coroutine WaveWaitCoroutine;

    // 呼叫此方法開始遊戲
    public void StartGame()
    {
        //currentWave = 0;
        UpdateWave();
        NextCoroutine = StartCoroutine(SpawnNextWave());
        //a =StartCoroutine(我是測試協成());
        start.SetActive(false);
        nextWave.SetActive(false);
        Time.timeScale = 1f;
    }

    public void InstantiateEnemy(GameObject monster, Vector3 spawnPosition)
    {
        GameObject monsterPrefab, rotatorPrefab; // 生成父物件暫存用
        monsterPrefab = Instantiate(monster, spawnPosition, Quaternion.identity);
        rotatorPrefab = Instantiate(monster.GetComponent<EnemyBasic>().monsterRotator, spawnPosition, Quaternion.identity);
        monsterPrefab.transform.parent = rotatorPrefab.transform;
    }

    // 呼叫此方法在怪物生成後，減去生成的數量，如果某種怪物的數量已經為0，表示不再生成此種怪物
    public void ReduceRemainingEnemies(int normal, int speed, int defend, int normalElite, int speedElite, int defendElite)
    {
        remainingEnemies[0] -= normal;
        remainingEnemies[1] -= speed;
        remainingEnemies[2] -= defend;
        remainingEnemies[3] -= normalElite;
        remainingEnemies[4] -= speedElite;
        remainingEnemies[5] -= defendElite;
    }

    // 呼叫此方法判斷是否還有剩餘怪物可以生成
    public bool HasRemainingEnemies()
    {
        foreach (int count in remainingEnemies)
        {
            if (count > 0)
            {
                return true;
            }
        }
        return false;
    }

    // 呼叫此方法在下一波的怪物數量為0時進行等待，然後生成下一波怪物
    public void WaveCompleted()
    {
        WaveWaitCoroutine= StartCoroutine(WaitAndSpawnNextWave());
    }

    // 等待一段時間後生成下一波怪物
    private IEnumerator WaitAndSpawnNextWave()
    {
        if (countdownTime > 0)
        {
            countdownStart = true;
            nextWave.SetActive(true);
            waveDisplay.SetActive(false);
        }
        yield return new WaitForSeconds(10f); // 等待10秒，你可以自行調整等待時間

        // 停止 SpawnNextWave 協程（如果在這期間已經啟動）
        StopCoroutine(NextCoroutine);
        NextCoroutine = StartCoroutine(SpawnNextWave());
        countdownTime = 10;
        countdownStart = false;
        nextWave.SetActive(false);
        UpdateWave();
    }

    #region 判斷生成小Boss
    private bool ShouldSpawnBoss(int waveNumber)
    {
        foreach (var bossData in bossSpawnData)
        {
            if (bossData.waveNumber == waveNumber)
            {
                return true;
            }
        }
        return false;
    }

    private GameObject GetBossPrefabForWave(int waveNumber)
    {
        foreach (var bossData in bossSpawnData)
        {
            if (bossData.waveNumber == waveNumber)
            {
                return bossData.bossPrefab;
            }
        }
        return null;
    }
    #endregion

    #region 更新錢包、波數、血量等
    ///更新金幣
    public void UpdateCoinWallet()
    {
        coinText.text = Coin.ToString();
    }

    public void UpdateWave()
    {
        waveText.text = "波數：" + (currentWave + 1) + "/" + wave;
        condensedWaveText.text = (currentWave + 1)+ "/" + wave;
    }

    public void UpdateCoreHP()
    {
        HPtext.text = CoreHP.ToString();
    }
    #endregion

    #region 按鈕類腳本，內包含暫停介面按鈕、下一波、回到主選單、重新遊玩
    bool OpenStopPanel = false;

    public void ClickStopButtom()
    {
        OpenStopPanel = !OpenStopPanel;
        pausePanel.SetActive(OpenStopPanel);

        if (OpenStopPanel)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    public void ClickNextWave()
    {
        print("開始下一波");
        //暫停倒數下一波協成
        StopCoroutine(WaveWaitCoroutine);
        StopCoroutine(NextCoroutine);
        UpdateWave();
        countdownTime = 10;
        countdownStart = false;
        nextWave.SetActive(false);
        waveDisplay.SetActive(true);
        NextCoroutine = StartCoroutine(SpawnNextWave());
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(levelData.level);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    #endregion
}
