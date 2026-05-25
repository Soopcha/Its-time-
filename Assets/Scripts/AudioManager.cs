using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Музыка")]
    public AudioClip MenuMusic;
    public AudioClip GameMusic;

    [Header("Настройки")]
    [Range(0f, 1f)]
    public float MusicVolume = 0.5f;

    private AudioSource musicSource;

    void Awake()
    {
        // Singleton (не уничтожается при смене сцен)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Создаём AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = MusicVolume;
    }

    public void PlayMenuMusic()
    {
        if (MenuMusic != null && musicSource.clip != MenuMusic)
        {
            musicSource.clip = MenuMusic;
            musicSource.Play();
        }
    }

    public void PlayGameMusic()
    {
        if (GameMusic != null && musicSource.clip != GameMusic)
        {
            musicSource.clip = GameMusic;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        musicSource.volume = MusicVolume;
    }
}