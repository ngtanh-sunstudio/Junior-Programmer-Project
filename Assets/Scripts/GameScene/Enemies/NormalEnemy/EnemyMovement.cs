using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float zRange = 30f;

    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();

        Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            enemyRigidbody.isKinematic = true;
        }
    }

    private void Update()
    {
        Move();
        ReturnToPoolOutOfBounds();
    }

    private void Move()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
    }

    private void ReturnToPoolOutOfBounds()
    {
        if (Mathf.Abs(transform.position.z) > zRange)
        {
            enemyController.ReturnToPool();
        }
    }
}
