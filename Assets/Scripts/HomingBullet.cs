using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 10f;
    public float rotateSpeed = 5f;
    public float lifeTime = 5f;

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        FindClosestEnemy();
    }
    void Update()
    {
        if (target == null)
        {
            return;
        }
        Vector3 direction =
            (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
    private void FindClosestEnemy()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = 
                Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = enemy.transform;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
