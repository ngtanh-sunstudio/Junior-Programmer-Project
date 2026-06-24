using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerWeapon))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private InputSystem_Actions inputActions;
    private PlayerHealth health;
    private PlayerMovement movement;
    private PlayerWeapon weapon;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        health = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<PlayerWeapon>();
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;
        inputActions.Player.Enable();
    }

    private void Update()
    {
        if (inputActions.Player.Pause.WasPressedThisFrame() && gameManager != null)
        {
            gameManager.TogglePause();
        }

        if (gameManager != null && gameManager.IsPaused)
        {
            return;
        }

        movement.Move(inputActions.Player.Move.ReadValue<Vector2>());

        if (inputActions.Player.Attack.WasPressedThisFrame())
        {
            weapon.TryFire();
        }
    }

    public void TakeProjectileDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    public void SetShielded(bool value)
    {
        health.SetShielded(value);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        movement.SetSpeedMultiplier(multiplier);
    }

    public void SetMultifiring(bool value)
    {
        weapon.SetMultifiring(value);
    }

    private void HandleDeath()
    {
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Died -= HandleDeath;
        }

        if (inputActions != null)
        {
            inputActions.Player.Disable();
        }
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}
