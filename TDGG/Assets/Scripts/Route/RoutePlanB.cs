using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePlanB : MonoBehaviour
{
    public static Transform[] pointB;

    void Awake() 
    {
        pointB = new Transform[transform.childCount]; // 取得路徑點個數
        for (int i = 0; i < pointB.Length; i++)
        {
            pointB[i] = transform.GetChild(i); // 取得路徑點
        }
    }
}
