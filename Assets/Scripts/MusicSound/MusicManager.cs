using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : Singleton<MusicManager> {

    public AudioClip[] music;
    public AudioMixerGroup mixerGroup;

    private int musicAt;
    private bool fading = false;
    private AudioSource audioSource;

    void Awake() {
        _instance = this;
        DontDestroyOnLoad(this);

        audioSource = GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixerGroup;

        musicAt = music.Length;

        StartCoroutine(PlayMusicIfEnded());
    }

    private IEnumerator PlayMusicIfEnded() {
        while (true) {
            if (!audioSource.isPlaying) {
                PlayNewMusic();
            }

            yield return null;
        }
    }

    private IEnumerator FadeIn(float FadeTime) {
        if (!audioSource.isPlaying) audioSource.Play();

        fading = true;
        float endVolume = PreferencesScript.Instance.GetNomalizedMusicVolume();

        while (audioSource.volume < endVolume) {
            audioSource.volume += endVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        fading = false;
        audioSource.volume = endVolume;
    }

    /// <summary>
    /// Plays next music from array. If gone through the array it shuffles it then plays a new music from it.
    /// </summary>
    private void PlayNewMusic() {
        // Short summary: it plays the music array in the order it is in the array. But after playing thorug it
        // and the first play it shuffles it.

        musicAt++;

        if (musicAt >= music.Length) { 
            new System.Random().Shuffle(music);

            musicAt = 0;
        }

        audioSource.clip = music[musicAt];
        audioSource.Play();

        audioSource.volume = 0f; // for fading
        fading = true;
        StartCoroutine(FadeIn(10f));
    }

    public void SetVolume(float volume) {
        if (!fading) { 
            audioSource.volume = volume;
        }
    }
}

static class RandomExtensions {
    public static void Shuffle<T>(this System.Random rand, T[] array) {
        int n = array.Length;
        while (n > 1) {
            int k = rand.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}