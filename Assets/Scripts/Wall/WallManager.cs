using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public WallType wallType;
    public int HP;

    private void Start()
    {
        if (wallType == WallType.BurnWall)
        {
            HP = 1;
        }

        else if (wallType == WallType.NormalWall)
        {
            HP = 5;
        }

        else if (wallType == WallType.PoisonWall)
        {
            HP = 2;
        }

        else
        {
            print("我沒抓到城牆Type");
        }
    }

    private void Update()
    {
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
