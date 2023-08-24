using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    private List<GameObject> enemyList = new List<GameObject>();
    private readonly string enemyLayer = "Monster";
    private float shootInterval = .1f;
    private float bulletSpeed = 1f;
    private bool isShooting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayer))
        {
            enemyList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(enemyLayer))
        {
            enemyList.Remove(other.gameObject);
        }
    }

    private void Update()
    {
        if (enemyList.Count > 0 && !isShooting)
        {
            StartCoroutine(Shoot());
        }

        if (enemyList.Count > 0 && enemyList[0] == null)
        {
            enemyList.Remove(enemyList[0]);
        }
    }

    private IEnumerator Shoot()
    {
        isShooting = true;

        while (enemyList.Count > 0)
        {
            GameObject targetEnemy = enemyList[0];
            Vector3 direction = targetEnemy.transform.position - transform.position;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            StartCoroutine(MoveBullet(bullet, direction));

            yield return new WaitForSeconds(shootInterval);
        }

        isShooting = false;
    }

    private IEnumerator MoveBullet(GameObject bullet, Vector3 direction)
    {
        while (Vector3.Distance(bullet.transform.position, transform.position) < Vector3.Distance(direction, Vector3.zero))
        {
            bullet.transform.Translate(bulletSpeed * Time.deltaTime * direction.normalized);
            yield return null;
        }

        Destroy(bullet);
    }
}
