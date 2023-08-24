using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private const string monsterLayer = "Monster";
    private const string floorTag = "Floor";
    public bool isHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (IsMonsterHit(other))
        {
            HitMonster(other);
        }
        else if (IsFloorHit(other))
        {
            Destroy(gameObject);
        }
    }

    private bool IsMonsterHit(Collider other)
    {
        return other.gameObject.layer == LayerMask.NameToLayer(monsterLayer) && other.TryGetComponent(out EnemyBasic enemy);
    }

    private void HitMonster(Collider other)
    {
        EnemyBasic enemy = other.GetComponent<EnemyBasic>();
        enemy.HP -= 5;
        Destroy(gameObject);
        isHit = true;
        Debug.Log($"À»¤¤¼Ä¤H¡I Hp: {enemy.HP}");
    }

    private bool IsFloorHit(Collider other)
    {
        return other.CompareTag(floorTag);
    }

    private void Update()
    {
        if (gameObject.transform.position.y <= 0)
        {
            Destroy(gameObject);
        }
    }
}
