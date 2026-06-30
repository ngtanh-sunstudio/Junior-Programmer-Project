using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonPersistent<AudioManager>
{
    private const string SFXVolumePreferenceKey = "Audio.SFXVolume";
    private const string MusicVolumePreferenceKey = "Audio.MusicVolume";
    private const float PreferenceSaveDelaySeconds = 0.25f;

    [Header("SFX")]
    [FormerlySerializedAs("buttonClickSFX")]
    [SerializeField] private AudioClip defaultButtonClickSFX;
    [SerializeField] private AudioClip confirmButtonClickSFX;
    [SerializeField] private AudioClip backButtonClickSFX;
    [SerializeField, Range(0f, 1f)] private float sfxVolume;
    [SerializeField] private AudioSource SFXSource;

    [Header("Music")]
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip gameBGM;
    [SerializeField] private AudioClip endingBGM;
    [SerializeField, Range(0f, 1f)] private float musicVolume;
    [SerializeField] private AudioSource musicSource;

    [Header("Configuration")]
    [SerializeField] private int titleSceneIndex;

    public float SFXVolume => sfxVolume;
    public float MusicVolume => musicVolume;

    private Coroutine savePreferencesCoroutine;

    protected override void Awake()
    {
        base.Awake();

        if (!IsSingletonInstance)
        {
            return;
        }

        if (!ValidateSerializedReferences())
        {
            ReleaseSingletonInstance();
            Destroy(gameObject); // Destroyed to prevent an orphan object
            return;
        }

        LoadVolumeSettings();
        ApplySFXVolume(sfxVolume);
        ApplyMusicVolume(musicVolume);

        SettingsOverlay.SFXVolumeChanged += SetSFXVolume;
        SettingsOverlay.MusicVolumeChanged += SetMusicVolume;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        UIButtonSoundEmitter.Clicked += PlayButtonClick;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene());
    }

    public void PlayEndingBGM()
    {
        PlayBGM(endingBGM);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene);
    }

    private void PlayMusicForScene(Scene scene)
    {
        AudioClip sceneMusic = scene.buildIndex == titleSceneIndex
            ? titleBGM
            : gameBGM;

        PlayBGM(sceneMusic);
    }

    private void PlayBGM(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void SetSFXVolume(float volume)
    {
        ApplySFXVolume(volume);
        PlayerPrefs.SetFloat(SFXVolumePreferenceKey, sfxVolume);
        SchedulePreferencesSave();
    }

    private void SetMusicVolume(float volume)
    {
        ApplyMusicVolume(volume);
        PlayerPrefs.SetFloat(MusicVolumePreferenceKey, musicVolume);
        SchedulePreferencesSave();
    }

    private void LoadVolumeSettings()
    {
        sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(SFXVolumePreferenceKey, sfxVolume));
        musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumePreferenceKey, musicVolume));
    }

    private void ApplySFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SFXSource.volume = sfxVolume;
    }

    private void ApplyMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    private void SchedulePreferencesSave()
    {
        if (savePreferencesCoroutine != null)
        {
            StopCoroutine(savePreferencesCoroutine);
        }

        savePreferencesCoroutine = StartCoroutine(SavePreferencesAfterDelay());
    }

    private IEnumerator SavePreferencesAfterDelay()
    {
        yield return new WaitForSecondsRealtime(PreferenceSaveDelaySeconds);

        savePreferencesCoroutine = null;
        PlayerPrefs.Save();
    }

    private void FlushPreferences()
    {
        if (savePreferencesCoroutine != null)
        {
            StopCoroutine(savePreferencesCoroutine);
            savePreferencesCoroutine = null;
        }

        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused && IsSingletonInstance)
        {
            FlushPreferences();
        }
    }

    private void OnApplicationQuit()
    {
        if (IsSingletonInstance)
        {
            FlushPreferences();
        }
    }

    protected override void OnDestroy()
    {
        if (IsSingletonInstance)
        {
            FlushPreferences();
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SettingsOverlay.SFXVolumeChanged -= SetSFXVolume;
            SettingsOverlay.MusicVolumeChanged -= SetMusicVolume;
            UIButtonSoundEmitter.Clicked -= PlayButtonClick;
        }

        base.OnDestroy();
    }

    public void PlaySFX(AudioClip audioClip)
    {
        SFXSource.PlayOneShot(audioClip);
    }

    public void PlayButtonClick(UIButtonSoundType soundType)
    {
        AudioClip clip = soundType switch
        {
            UIButtonSoundType.Confirm => confirmButtonClickSFX,
            UIButtonSoundType.Back => backButtonClickSFX,
            _ => defaultButtonClickSFX
        };

        PlaySFX(clip);
    }

    private bool ValidateSerializedReferences()
    {
        bool hasReferences = true;

        if (SFXSource == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing an SFX source reference.", this);
            hasReferences = false;
        }

        if (musicSource == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing a music source reference.", this);
            hasReferences = false;
        }

        if (defaultButtonClickSFX == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing the default button click SFX clip.", this);
            hasReferences = false;
        }

        if (confirmButtonClickSFX == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing the confirm button click SFX clip.", this);
            hasReferences = false;
        }

        if (backButtonClickSFX == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing the back button click SFX clip.", this);
            hasReferences = false;
        }

        if (titleBGM == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing a title BGM clip.", this);
            hasReferences = false;
        }

        if (gameBGM == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing a game BGM clip.", this);
            hasReferences = false;
        }

        if (endingBGM == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing an ending BGM clip.", this);
            hasReferences = false;
        }

        return hasReferences;
    }
}
