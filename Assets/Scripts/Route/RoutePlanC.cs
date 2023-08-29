using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePlanC : MonoBehaviour
{
    public static Transform[] pointC;

    void Awake()
    {
        pointC = new Transform[transform.childCount]; // 取得路徑點個數
        for (int i = 0; i < pointC.Length; i++)
        {
            pointC[i] = transform.GetChild(i); // 取得路徑點
        }
    }
}
