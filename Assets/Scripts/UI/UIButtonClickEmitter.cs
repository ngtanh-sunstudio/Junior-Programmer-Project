using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonClickEmitter : MonoBehaviour
{
    public static event Action Clicked;

    [SerializeField] private Button button;

    private bool isRegistered;

    private void Awake()
    {
        if (button == null)
        {
            Debug.LogError($"{nameof(UIButtonClickEmitter)} is missing a button reference.", this);
            enabled = false;
            return;
        }

        button.onClick.AddListener(HandleClicked);
        isRegistered = true;
    }

    private void OnDestroy()
    {
        if (!isRegistered)
        {
            return;
        }

        button.onClick.RemoveListener(HandleClicked);
    }

    private void HandleClicked()
    {
        Clicked?.Invoke();
    }
}
