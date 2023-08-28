using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Benchmark/LevelData")]

public class LevelData : ScriptableObject
{
    [Header("第幾關")]  //給UI抓取呈現說目前是第幾關的，可有可無。
    public int level;

    [Header("怪物總數量")]//在打總數量時需要再看該關是否有小Boss，要自行++
    public int enemyQuantity;

    [Header("該關卡出現正常型態怪物數量")]
    public int normalMinion;

    [Header("該關卡出現速度型態怪物數量")]
    public int speedMinion;

    [Header("該關卡出現防禦型態怪物數量")]
    public int defenseMinion;

    [Header("該關卡出現菁英正常型態怪物數量")]
    public int normalElite;

    [Header("該關卡出現菁英速度型態怪物數量")]
    public int speedElite;

    [Header("該關卡出現菁英防禦型態怪物數量")]
    public int defenseElite;

    [Header("該關卡出現的小Boss數量")]//直接將三種型態寫在一起，如果該關沒有全部輸入0
    public int attackSupreme, speedSupreme, defenseSupreme;

    [Header("該關卡出現的Boss")]//要在Boss身上新增另外腳本，當Boss被打死關卡結束
    public GameObject boss; //數值直接在Boss身上調整

    [Header("關卡波數")]
    public int wave;

    [Header("波數怪物數量陣列")]
    public int[] enemyWave;

    [Header("關卡核心血量")]
    public int coreHP = 10;

    [Header("初始金錢")]
    public int coin;
}
