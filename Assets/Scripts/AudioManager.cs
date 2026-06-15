using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Музыка")]
    public AudioClip MenuMusic;
    public AudioClip GameMusic;

    private AudioSource musicSource;
    private AudioSource sfxSource;

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

        // Создаём AudioSource для музыки
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // Создаём AudioSource для звуков
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        // Загружаем сохранённую громкость
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        Debug.Log($" AudioManager: Музыка={musicVolume}, Звуки={sfxVolume}");
    }

    public void PlayMenuMusic()
    {
        if (MenuMusic != null && musicSource.clip != MenuMusic)
        {
            musicSource.clip = MenuMusic;
            musicSource.Play();
            Debug.Log(" Играет музыка меню");
        }
    }

    public void PlayGameMusic()
    {
        if (GameMusic != null && musicSource.clip != GameMusic)
        {
            musicSource.clip = GameMusic;
            musicSource.Play();
            Debug.Log(" Играет игровая музыка");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Воспроизведение звукового эффекта
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Установка громкости музыки
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        Debug.Log($"Громкость музыки: {volume}");
    }

    // Установка громкости звуков
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        Debug.Log($"Громкость звуков: {volume}");
    }

    // Получить текущую громкость музыки
    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    // Получить текущую громкость звуков
    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }
}