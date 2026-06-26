using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsOverlay : MonoBehaviour
{
    public static event Action<float> SFXVolumeChanged;
    public static event Action<float> MusicVolumeChanged;

    [SerializeField] private MenuController menuControllerScript;
    [SerializeField] private Button backButton;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider musicSlider;

    private bool hasValidReferences;
    private bool sliderListenersRegistered;

    void Awake()
    {
        hasValidReferences = ValidateSerializedReferences();
        if (!hasValidReferences)
        {
            enabled = false;
            return;
        }

        HandleCloseSettings();
    }

    private void Start()
    {
        if (!hasValidReferences)
        {
            return;
        }

        InitializeSliders();
        RegisterSliderListeners();
    }

    private void HandleCloseSettings()
    {
        backButton.onClick.AddListener(menuControllerScript.CloseSettings);
    }

    private void InitializeSliders()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError($"{nameof(SettingsOverlay)} could not initialize sliders because no {nameof(AudioManager)} instance exists.", this);
            return;
        }

        SFXSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
    }

    private void RegisterSliderListeners()
    {
        SFXSlider.onValueChanged.AddListener(HandleSFXVolumeChanged);
        musicSlider.onValueChanged.AddListener(HandleMusicVolumeChanged);
        sliderListenersRegistered = true;
    }

    private void OnDestroy()
    {
        if (!sliderListenersRegistered)
        {
            return;
        }

        SFXSlider.onValueChanged.RemoveListener(HandleSFXVolumeChanged);
        musicSlider.onValueChanged.RemoveListener(HandleMusicVolumeChanged);
    }

    private void HandleSFXVolumeChanged(float volume)
    {
        SFXVolumeChanged?.Invoke(volume);
    }

    private void HandleMusicVolumeChanged(float volume)
    {
        MusicVolumeChanged?.Invoke(volume);
    }

    private bool ValidateSerializedReferences()
    {
        bool hasReferences = true;

        if (menuControllerScript == null)
        {
            Debug.LogError($"{nameof(SettingsOverlay)} is missing a menu controller reference.", this);
            hasReferences = false;
        }

        if (backButton == null)
        {
            Debug.LogError($"{nameof(SettingsOverlay)} is missing a back button reference.", this);
            hasReferences = false;
        }

        if (SFXSlider == null)
        {
            Debug.LogError($"{nameof(SettingsOverlay)} is missing an SFX slider reference.", this);
            hasReferences = false;
        }

        if (musicSlider == null)
        {
            Debug.LogError($"{nameof(SettingsOverlay)} is missing a music slider reference.", this);
            hasReferences = false;
        }

        return hasReferences;
    }
}
