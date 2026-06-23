using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>(true);
        }
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (slider == null)
        {
            return;
        }

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetHealth(int health)
    {
        if (slider == null)
        {
            return;
        }

        slider.value = health;
    }
}
