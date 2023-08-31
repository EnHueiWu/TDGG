using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//怪物型態
public enum EnemyType       //此種類用來計算關卡怪物出來的是哪種來去減掉關卡的資料結構。
{
    None,
    Normal,
    Speed,
    Defense,
    Attack,
}

//怪物種類
public enum MonsterType     //此種類用來判斷關卡Boss過關，以及金幣獲取量
{
    None,
    Boss,
    Supreme,
    Elite,
    Minion,
}

//城牆種類
public enum WallType
{
    None,
    BurnWall,
    PoisonWall,
    NormalWall
}

//子彈種類
public enum BulletType
{
    None,
    NormalBullet,
    AdvancedBullet,
    PoisonBullet,
    IceBullet
}