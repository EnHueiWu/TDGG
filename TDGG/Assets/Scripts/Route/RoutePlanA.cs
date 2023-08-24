using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePlanA : MonoBehaviour
{
    public static Transform[] pointA;

    void Awake() 
    {
        pointA = new Transform[transform.childCount]; // 取得路徑點個數
        for (int i = 0; i < pointA.Length; i++)
        {
            pointA[i] = transform.GetChild(i); // 取得路徑點
        }
    }
}
