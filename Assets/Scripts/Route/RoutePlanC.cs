using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePlanC : MonoBehaviour
{
    public static Transform[] pointC;

    void Awake()
    {
        pointC = new Transform[transform.childCount]; // ���o���|�I�Ӽ�
        for (int i = 0; i < pointC.Length; i++)
        {
            pointC[i] = transform.GetChild(i); // ���o���|�I
        }
    }
}
