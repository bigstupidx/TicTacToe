using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class CellAudioSourcePool {

    private static Stack<AudioSource> audioSourceStack;

    private static AudioMixerGroup mixer;

    static CellAudioSourcePool() {
        audioSourceStack = new Stack<AudioSource>();
        mixer = Resources.Load<AudioMixer>("AudioMixer/GameAudioMixer").FindMatchingGroups("PlaceSounds")[0];
    }

    public static AudioSource GetAudioSource() {
        if (audioSourceStack.Count == 0) {
            AudioSource audio = new AudioSource();

            audio.playOnAwake = false;
            audio.outputAudioMixerGroup = mixer;
        }

        return audioSourceStack.Pop();
    }

    public static void PoolAudioSource(AudioSource source) {
        source.clip = null;
        audioSourceStack.Push(source);
    }

}
