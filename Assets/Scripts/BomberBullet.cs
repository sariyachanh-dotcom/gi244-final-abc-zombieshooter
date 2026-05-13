using UnityEngine;

public class BomberBullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public float explosionRadius = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Collider[] enemies =
                Physics.OverlapSphere(
                    transform.position, explosionRadius );

            foreach (Collider hit in enemies)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Destroy (hit.gameObject);
                }
            }
            Destroy(gameObject);
        }
    }
}
