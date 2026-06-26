using System.Collections;
using UnityEngine;

public class BossZoneAttack : MonoBehaviour
{
    [SerializeField] private GameObject[] zones;
    [SerializeField] private Transform zoneRoot;
    [SerializeField] private float attackInterval = 5f;
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private int damage = 2;

    private void Awake()
    {
        if (zones == null || zoneRoot == null || zones.Length == 0)
        {
            Debug.LogWarning("Boss zone attack has no zones assigned.", this);
            enabled = false;
            return;
        }

        zoneRoot.SetParent(null, false);
        zoneRoot.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        zoneRoot.localScale = Vector3.one;

        foreach (GameObject zone in zones)
        {
            SetZonesActive(zone, false);
        }
    }

    private void Start()
    {
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            int randomIndex = Random.Range(0, zones.Length);
            GameObject selectedZone = zones[randomIndex];
            if (selectedZone == null)
            {
                Debug.LogWarning(
                    "Boss zone attack contains an empty zone slot.",
                    this
                );
                continue;
            }

            SetZonesActive(selectedZone, true);

            yield return new WaitForSeconds(warningDuration);

            DamagePlayerInside(selectedZone);
            SetZonesActive(selectedZone, false);
        }
    }

    private void SetZonesActive(GameObject zone, bool active)
    {
        if (zone != null)
        {
            zone.SetActive(active);
        }
    }

    private void DamagePlayerInside(GameObject zone)
    {
        if (zone == null ||
            !zone.TryGetComponent(out BoxCollider boxCollider))
        {
            Debug.LogWarning("Boss attack zone requires a BoxCollider.", zone);
            return;
        }

        // bounds.extents contains the collider's world-space half-size.
        // This is accurate because zones remain axis-aligned (90-degree rotations).
        // For arbitrary rotations, use size, lossyScale, and rotation instead.
        Collider[] hits = Physics.OverlapBox(
            boxCollider.bounds.center,
            boxCollider.bounds.extents,
            Quaternion.identity
        );

        foreach (Collider hit in hits)
        {
            PlayerHealth player = hit.GetComponentInParent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
                return;
            }
        }
    }

    private void OnDisable()
    {
        if (zones == null)
        {
            Debug.LogError($"{nameof(BossZoneAttack)} cannot disable attack zones because no zones are configured.", this);
            return;
        }

        foreach (GameObject zone in zones)
        {
            SetZonesActive(zone, false);
        }
    }

    private void OnDestroy()
    {
        if (zoneRoot != null)
        {
            Destroy(zoneRoot.gameObject);
        }
    }
}
