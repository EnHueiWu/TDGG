using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Benchmark/EnemyData")]

public class EnemyData : ScriptableObject
{
    [Header("怪物名稱")]
    public string enemyName;

    [Header("怪物血量"), Range(0, 2000)]
    public float HP;

    [Header("移動速度"), Range(0, 10)]
    public float speed;
}
