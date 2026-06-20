using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources — add 2 AudioSource components, drag here")]
    public AudioSource musicSource;   
    public AudioSource sfxSource;    

    [Header("Music clips")]
    public AudioClip backgroundMusic; 

    [Header("SFX clips")]
    public AudioClip sfxPackOpen;
    public AudioClip sfxWin;
    public AudioClip sfxLose;
    public AudioClip sfxButtonClick;
    public AudioClip sfxCardHit;      
    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.4f;
    [Range(0f, 1f)] public float sfxVolume   = 0.7f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null)
        {
            musicSource.loop   = true;
            musicSource.volume = musicVolume;
            if (backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }

        if (sfxSource != null)
        {
            sfxSource.loop   = false;
            sfxSource.volume = sfxVolume;
        }
    }

    public void PlayPackOpen()    => PlaySfx(sfxPackOpen);
    public void PlayWin()         => PlaySfx(sfxWin);
    public void PlayLose()        => PlaySfx(sfxLose);
    public void PlayButtonClick() => PlaySfx(sfxButtonClick);
    public void PlayCardHit()     => PlaySfx(sfxCardHit);

    void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    public void StopMusic()  { if (musicSource != null) musicSource.Stop(); }
    public void StartMusic() { if (musicSource != null && !musicSource.isPlaying) musicSource.Play(); }
}
