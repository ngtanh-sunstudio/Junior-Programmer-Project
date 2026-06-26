using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClickSFX;
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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!ValidateSerializedReferences())
        {
            Instance = null;
            enabled = false;
            return;
        }

        SetSFXVolume(sfxVolume);
        SetMusicVolume(musicVolume);

        SettingsOverlay.SFXVolumeChanged += SetSFXVolume;
        SettingsOverlay.MusicVolumeChanged += SetMusicVolume;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        UIButtonClickEmitter.Clicked += PlayButtonClick;
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
        sfxVolume = volume;
        SFXSource.volume = volume;
    }

    private void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume;
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SettingsOverlay.SFXVolumeChanged -= SetSFXVolume;
        SettingsOverlay.MusicVolumeChanged -= SetMusicVolume;
        UIButtonClickEmitter.Clicked -= PlayButtonClick;

        Instance = null;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        SFXSource.PlayOneShot(audioClip);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
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

        if (buttonClickSFX == null)
        {
            Debug.LogError($"{nameof(AudioManager)} is missing a button click SFX clip.", this);
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
