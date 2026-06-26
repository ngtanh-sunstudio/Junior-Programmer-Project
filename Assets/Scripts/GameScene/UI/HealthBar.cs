using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (slider == null)
        {
            Debug.LogError($"{nameof(HealthBar)} is missing a slider reference.", this);
        }
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (slider == null)
        {
            Debug.LogError($"{nameof(HealthBar)} cannot set max health because the slider reference is missing.", this);
            return;
        }

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetHealth(int health)
    {
        if (slider == null)
        {
            Debug.LogError($"{nameof(HealthBar)} cannot set health because the slider reference is missing.", this);
            return;
        }

        slider.value = health;
    }
}
