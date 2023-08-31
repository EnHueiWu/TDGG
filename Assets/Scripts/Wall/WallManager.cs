using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public WallType wallType;
    public GameObject builtGrid;
    public int HP;

    private void Awake()
    {
        switch(wallType)
        {
            case WallType.BurnWall:
                HP = 1;
                break;
            
            case WallType.NormalWall:
                HP = 5;
                break;
            
            case WallType.PoisonWall:
                HP = 2;
                break;
            
            default:
                Debug.LogWarning("Wall type not found!");
                break;
        }
    }

    private void Update()
    {
        if (HP <= 0)
        {
            Destroy(gameObject);
            builtGrid.GetComponent<HoverAnimation>().isBulit = false;
        }
    }
}
