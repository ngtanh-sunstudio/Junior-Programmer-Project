using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private readonly List<Button> registeredButtons = new List<Button>();

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField, Range(0f, 1f)] private float SFXVolume;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private AudioSource SFXSource;

    [Header("Music")]
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip gameBGM;
    [SerializeField] private AudioClip endingBGM;
    [SerializeField, Range(0f, 1f)] private float musicVolume;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private AudioSource musicSource;

    [Header("Configuration")]
    [SerializeField, Range(0f, 1f)] private float defaultVolume = 0.1f;
    [SerializeField] private int titleSceneIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.BindSliders(SFXSlider, musicSlider);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        if (SFXVolume == 0f)
        {
            SFXVolume = defaultVolume;
        }

        if (musicVolume == 0f)
        {
            musicVolume = defaultVolume;
        }

        BindSliders(SFXSlider, musicSlider);
        SetSFXVolume(SFXVolume);
        SetMusicVolume(musicVolume);
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene());
        RegisterButtonSounds();
    }

    public void PlayEndingBGM()
    {
        PlayBGM(endingBGM);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene);
        UnregisterButtonSounds(); // cleanup when changing scenes

        RegisterButtonSounds();
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
        if (musicSource == null || clip == null)
        {
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void BindSliders(Slider sfxSlider, Slider newMusicSlider)
    {
        if (SFXSlider != null)
        {
            SFXSlider.onValueChanged.RemoveListener(SetSFXVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        }

        SFXSlider = sfxSlider;
        musicSlider = newMusicSlider;

        if (SFXSlider != null)
        {
            SFXSlider.SetValueWithoutNotify(SFXVolume);
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(musicVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    private void SetSFXVolume(float volume)
    {
        SFXVolume = volume;

        if (SFXSource != null)
        {
            SFXSource.volume = volume;
        }
    }

    private void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    private void RegisterButtonSounds()
    {
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if (registeredButtons.Contains(button))
            {
                continue;
            }
            
            registeredButtons.Add(button);
            button.onClick.AddListener(PlayButtonClick);
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (SFXSlider != null)
        {
            SFXSlider.onValueChanged.RemoveListener(SetSFXVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        }
        UnregisterButtonSounds();

        Instance = null;
    }

    private void UnregisterButtonSounds()
    {
        foreach (Button button in registeredButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveListener(PlayButtonClick);
            }
        }
        registeredButtons.Clear();
    }


    public void PlaySFX(AudioClip audioClip)
    {
        if (SFXSource != null && audioClip != null)
        {
            SFXSource.PlayOneShot(audioClip);
        }
    }

    public void PlayButtonClick()
    {
        if (SFXSource != null && buttonClickSFX != null)
        {
            SFXSource.PlayOneShot(buttonClickSFX);
        }
    }
}
