using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float xRange = 15f;
    [SerializeField] private float zUpperBound = 15f;
    [SerializeField] private float zLowerBound;

    private Rigidbody playerRigidbody;
    private float speedMultiplier = 1f;

    public Vector3 Velocity => playerRigidbody != null
        ? playerRigidbody.linearVelocity
        : Vector3.zero;

    public event Action SpedUp;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        transform.Translate(direction * speed * speedMultiplier * Time.deltaTime);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        if (multiplier > 1f)
        {
            SpedUp?.Invoke();
        }
        speedMultiplier = Mathf.Max(0f, multiplier);
    }

    public void StopPhysicsDrift()
    {
        if (playerRigidbody == null)
        {
            Debug.LogError($"{nameof(PlayerMovement)} cannot stop physics drift because the rigidbody reference is missing.", this);
            return;
        }

        playerRigidbody.linearVelocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -xRange, xRange);
        position.z = Mathf.Clamp(position.z, zLowerBound, zUpperBound);
        transform.position = position;

        StopPhysicsDrift();
    }
}
