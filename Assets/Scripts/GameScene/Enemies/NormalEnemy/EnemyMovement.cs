using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float zRange = 30f;

    private void Awake()
    {
        Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            enemyRigidbody.isKinematic = true;
        }
    }

    private void Update()
    {
        Move();
        DestroyOutOfBounds();
    }

    private void Move()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
    }

    private void DestroyOutOfBounds()
    {
        if (Mathf.Abs(transform.position.z) > zRange)
        {
            Destroy(gameObject);
        }
    }
}
