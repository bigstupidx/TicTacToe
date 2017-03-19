using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Random Audio")]
public class RandomSound : ScriptableObject {

    public AudioClip[] clips;

    public bool randomPitch = true;
    [MinMaxSlider(0f, 2f)]
    public Vector2 pitch;

    public bool randomVolume = true;
    [MinMaxSlider(0f, 1f)]
    public Vector2 volume;

    public AudioMixerGroup mixerGroup;

    private int lastIndex;

    /// <summary>
    /// Plays a random audio.
    /// </summary>
    public void PlayRandomAudio(AudioSource source) {
        PlayAudio(source, Random.Range(0, clips.Length));
    }

    /// <summary>
    /// Plays a random audio that's not the same as the last one.
    /// </summary>
    public void PlayRandomAudioNonRepeat(AudioSource source) {
        if (clips.Length == 1) PlayAudio(source, 0);
        else if (clips.Length == 2) {
            if (lastIndex == 0) PlayAudio(source, 1);
            else PlayAudio(source, 0);
        } else {
            int currIndex;
            do {
                currIndex = Random.Range(0, clips.Length);
            } while (currIndex == lastIndex);

            PlayAudio(source, currIndex);
        }
    }

    /// <summary>
    /// Plays audio with the index of at from the array
    /// </summary>
    private void PlayAudio(AudioSource source, int at) {
        lastIndex = at;

        if (randomPitch) source.pitch = Random.Range(pitch.x, pitch.y);
        if (randomVolume) source.volume = Random.Range(volume.x, volume.y) * PreferencesScript.Instance.GetSoundVolume() / 100f;
        source.clip = clips[at];
        source.outputAudioMixerGroup = mixerGroup;

        source.Play();
    }
	
}