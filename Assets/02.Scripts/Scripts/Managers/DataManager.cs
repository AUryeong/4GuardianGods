using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : SingletonBehavior<DataManager>
{
    protected override bool IsDontDestroying => true;

    [Header("Sound")]
    private const string SOUND_PATH = "Sounds/";
    private readonly Dictionary<string, AudioClip> audioClips = new();

    protected override void OnCreated()
    {
        LoadAudioClips();
    }

    private void LoadAudioClips()
    {
        var clips = Resources.LoadAll<AudioClip>(SOUND_PATH);
        foreach (var clip in clips)
            audioClips.Add(clip.name, clip);
    }

    public AudioClip GetAudioClip(string soundName)
    {
        audioClips.TryGetValue(soundName, out var audioClip);
        return audioClip;
    }
}
