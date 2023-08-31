using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public BulletType bulletType;
    public GameObject[] effect;
    public float shootInterval, bulletSpeed, damage;
    private const string monsterLayer = "Monster";
    private const string floorTag = "Floor";
    public bool isHit = false;

    private void Awake()
    {
        switch (bulletType)
        {
            case BulletType.NormalBullet:
                shootInterval = 0.1f;
                bulletSpeed = 1f;
                damage = 5f;
                break;

            case BulletType.AdvancedBullet:
                shootInterval = 0.09f;
                bulletSpeed = 1.1f;
                damage = 7f;
                break;

            case BulletType.PoisonBullet:
                shootInterval = 0.1f;
                bulletSpeed = 1f;
                damage = 3f;
                break;

            case BulletType.IceBullet:
                shootInterval = 0.2f;
                bulletSpeed = 0.5f;
                damage = 1f;
                break;

            default:
                Debug.LogWarning("Bullet type not found!");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsMonsterHit(other))
            HitMonster(other);

        else if (IsFloorHit(other))
            Destroy(gameObject);
    }

    private bool IsMonsterHit(Collider other)
    {
        return other.gameObject.layer == LayerMask.NameToLayer(monsterLayer) && other.TryGetComponent(out EnemyManager _);
    }

    private void HitMonster(Collider other)
    {
        EnemyManager enemy = other.GetComponent<EnemyManager>();
        GameObject effect = new();

        switch (bulletType)
        {
            case BulletType.NormalBullet:
                enemy.HP -= damage;
                break;

            case BulletType.AdvancedBullet:
                enemy.HP -= damage * enemy.hitcount;
                if (enemy.hitcount < 10) enemy.hitcount++;
                break;

            case BulletType.PoisonBullet:
                enemy.HP -= damage;
                enemy.isPoisoning = true;
                enemy.poisonCount = 5;
                break;

            case BulletType.IceBullet:
                enemy.HP -= damage;
                enemy.speed *= 0.5f;
                enemy.frozenTime = 3;
                break;
        }

        if (bulletType == BulletType.AdvancedBullet)
        {
            if (enemy.hitcount <= 4) effect = Instantiate(this.effect[0], other.transform.position, Quaternion.identity);

            else if (enemy.hitcount <= 9) effect = Instantiate(this.effect[1], other.transform.position, Quaternion.identity);

            else if (enemy.hitcount > 9) effect = Instantiate(this.effect[2], other.transform.position, Quaternion.identity);
        }

        else
            effect = Instantiate(this.effect[0], other.transform.position, Quaternion.identity);

        Destroy(gameObject);
        Destroy(effect, 1f);
        isHit = true;
    }

    private bool IsFloorHit(Collider other)
    {
        return other.CompareTag(floorTag);
    }

    private void Update()
    {
        if (gameObject.transform.position.y <= 0)
            Destroy(gameObject);
    }
}
