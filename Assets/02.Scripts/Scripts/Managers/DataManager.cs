using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : Singleton<DataManager>
{
    [Header("Sound")]
    private const string SOUND_PATH = "Sounds/";
    private readonly Dictionary<string, AudioClip> audioClips = new();

    public void Init()
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
        Debug.Assert(audioClip != null);
        return audioClip;
    }
}
