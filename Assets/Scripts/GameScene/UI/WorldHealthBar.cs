using UnityEngine;

public class WorldHealthBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fillRenderer;

    private Transform fillTransform;
    private Vector3 fullScale;
    private Vector3 fullPosition;
    private float fullWidth;
    private int maxHealth = 1;

    public bool Initialize()
    {
        if (fillRenderer == null)
        {
            Debug.LogError($"{nameof(WorldHealthBar)} is missing a fill renderer reference.", this);
            return false;
        }

        if (fillRenderer.sprite == null)
        {
            Debug.LogError($"{nameof(WorldHealthBar)} requires a sprite on its fill renderer.", this);
            return false;
        }

        fillTransform = fillRenderer.transform;
        fullScale = fillTransform.localScale;
        fullPosition = fillTransform.localPosition;
        fullWidth = fillRenderer.sprite.bounds.size.x * fullScale.x;
        return true;
    }

    public void SetMaxHealth(int value)
    {
        maxHealth = Mathf.Max(1, value);
        SetHealth(maxHealth);
    }

    public void SetHealth(int value)
    {
        float healthRatio = Mathf.Clamp01((float)value / maxHealth);

        Vector3 fillScale = fullScale;
        fillScale.x *= healthRatio;
        fillTransform.localScale = fillScale;

        // Shift the center as the sprite shrinks so its left edge stays fixed.
        Vector3 fillPosition = fullPosition;
        fillPosition.x -= fullWidth * (1f - healthRatio) * 0.5f;
        fillTransform.localPosition = fillPosition;
        fillRenderer.enabled = healthRatio > 0f;
    }
}
