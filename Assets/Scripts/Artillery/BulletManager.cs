using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public BulletType bulletType;
    public GameObject effect;
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
        return other.gameObject.layer == LayerMask.NameToLayer(monsterLayer) && other.TryGetComponent(out EnemyBasic _);
    }

    private void HitMonster(Collider other)
    {
        EnemyBasic enemy = other.GetComponent<EnemyBasic>();

        switch (bulletType)
        {
            case BulletType.NormalBullet:
                enemy.HP -= damage;
                break;

            case BulletType.AdvancedBullet:
                enemy.HP -= damage * enemy.hitcount;
                enemy.hitcount++;
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

        var effect = Instantiate(this.effect, other.transform.position, Quaternion.identity);
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
