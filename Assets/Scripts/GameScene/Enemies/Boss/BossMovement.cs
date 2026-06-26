using UnityEngine;

[RequireComponent(typeof(BossWeapon))]
public class BossMovement : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float xRange = 15f;
    [SerializeField] private float targetZ = 15f;

    private BossWeapon weapon;
    private Vector3 targetPosition;
    private bool hasReachedBattlePosition;

    private void Awake()
    {
        weapon = GetComponent<BossWeapon>();
    }

    private void Start()
    {
        targetPosition = GetTargetAtCurrentX();
    }

    private void Update()
    {
        if (!hasReachedBattlePosition)
        {
            MoveToBattlePosition();
            return;
        }

        MoveSideToSide();
    }

    private void MoveToBattlePosition()
    {
        targetPosition = GetTargetAtCurrentX();
        MoveTowardsTarget();

        if (!HasReachedTargetPosition())
        {
            return;
        }

        hasReachedBattlePosition = true;
        PickNewXTarget();
    }

    private void MoveSideToSide()
    {
        if (weapon.IsFiring)
        {
            return;
        }

        MoveTowardsTarget();

        if (HasReachedTargetPosition())
        {
            weapon.TryStartFiring(PickNewXTarget);
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    private void PickNewXTarget()
    {
        float randomX = Random.Range(-xRange, xRange);
        targetPosition = new Vector3(randomX, transform.position.y, targetZ);
    }

    private Vector3 GetTargetAtCurrentX()
    {
        return new Vector3(transform.position.x, transform.position.y, targetZ);
    }

    private bool HasReachedTargetPosition()
    {
        return Vector3.Distance(transform.position, targetPosition) <= 0.01f;
    }
}
