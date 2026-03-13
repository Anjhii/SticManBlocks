using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Música (Music)")]
    public AudioClip inicioVF;
    public AudioClip level1VF;
    public AudioClip level2VF;
    public AudioClip level3VF;
    public AudioClip powerupVF;
    public AudioClip gameOverMusic;
    public AudioClip winMusic;

    [Header("Efectos de Sonido (SFX)")]
    public AudioClip jump;
    public AudioClip fallJump;
    public AudioClip boton;
    public AudioClip dano;
    public AudioClip money1;
    public AudioClip takePowerup;
    public AudioClip nextLevelPortal;

    private float defaultMusicVolume = 1f; 
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            if (musicSource != null) defaultMusicVolume = musicSource.volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // Si ya está sonando la misma canción...
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            // Detenemos cualquier fade out accidental y restauramos el volumen
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            musicSource.volume = defaultMusicVolume;
            return;
        }

        // Si cambiamos a una canción nueva, cancelamos fades anteriores
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        // RESTAURAMOS EL VOLUMEN (Vital para que no inicie en 0 después de un fade out)
        musicSource.volume = defaultMusicVolume; 
        
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void FadeOutMusic(float duration)
    {
        if (musicSource == null || !musicSource.isPlaying) return;

        // Detenemos cualquier fade anterior para no cruzar corrutinas
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = musicSource.volume;
        float timer = 0f;

        // Reducimos el volumen gradualmente hasta 0
        while (timer < duration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null; // Esperamos al siguiente frame
        }

        musicSource.volume = 0f;
        musicSource.Stop(); // Detenemos la pista completamente al llegar a 0
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        
        // PlayOneShot permite reproducir el mismo sonido varias veces a la vez sin cortarse
        sfxSource.PlayOneShot(clip);
    }

    // Helper para botones desde el Editor de Unity
    public void PlayButtonSFX()
    {
        PlaySFX(boton);
    }
}