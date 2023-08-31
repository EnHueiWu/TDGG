using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    private readonly List<GameObject> enemyList = new();
    public int price;
    private readonly string enemyLayer = "Monster";
    private readonly string rotatorTag = "Rotator";
    private bool isShooting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayer) && !other.CompareTag(rotatorTag)) enemyList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayer) && !other.CompareTag(rotatorTag)) enemyList.Remove(other.gameObject);
    }

    private void Update()
    {
        if (enemyList.Count > 0 && !isShooting) StartCoroutine(Shoot());

        if (enemyList.Count > 0 && enemyList[0] == null) enemyList.Remove(enemyList[0]);
    }

    private IEnumerator Shoot()
    {
        isShooting = true;

        while (enemyList.Count > 0 && enemyList[0] != null)
        {
            GameObject targetEnemy = enemyList[0];
            Vector3 direction = targetEnemy.transform.position - transform.position;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            StartCoroutine(MoveBullet(bullet, direction));

            yield return new WaitForSeconds(bullet.GetComponent<BulletManager>().shootInterval);
        }

        isShooting = false;
    }

    private IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
    {
        while (bullet != null && !bullet.GetComponent<BulletManager>().isHit)
        {
            bullet.transform.Translate(bullet.GetComponent<BulletManager>().bulletSpeed * Time.deltaTime * direction.normalized);
            yield return null;
        }
    }
}
