using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum UIButtonSoundType
{
    Default,
    Confirm,
    Back
}

[RequireComponent(typeof(Button))]
public class UIButtonSoundEmitter : MonoBehaviour
{
    public static event Action<UIButtonSoundType> Clicked;

    [SerializeField] private UIButtonSoundType soundType;

    private Button button;
    private bool isRegistered;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError($"{nameof(UIButtonSoundEmitter)} requires a {nameof(Button)} component.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (button == null || isRegistered)
        {
            return;
        }

        button.onClick.AddListener(HandleClicked);
        isRegistered = true;
    }

    private void OnDisable()
    {
        if (!isRegistered)
        {
            return;
        }

        button.onClick.RemoveListener(HandleClicked);
        isRegistered = false;
    }

    private void HandleClicked()
    {
        Clicked?.Invoke(soundType);
    }
}
