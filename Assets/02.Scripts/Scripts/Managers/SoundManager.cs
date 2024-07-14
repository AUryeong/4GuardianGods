using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public enum SoundType
{
    Bgm,
    Sfx,
    Max
}

[System.Serializable]
public struct SoundSettings
{
    private readonly bool muteVolume;
    private readonly float audioVolume;

    public float Volume => muteVolume ? 0 : audioVolume;

    public SoundSettings(bool muteVolume, float audioVolume)
    {
        this.muteVolume = muteVolume;
        this.audioVolume = audioVolume;
    }
}

public class SoundManager : SingletonBehavior<SoundManager>
{
    private readonly Dictionary<SoundType, SoundSettings> soundSettingDict = new();
    private readonly List<AudioSource> bgmAudioSources = new(2);
    private int bgmPlayIndex;
    private ListableObjectPool<AudioSource> sfxAudioSources;

    protected override void OnCreated()
    {
        base.OnCreated();

        //TODO 설정 적용
        soundSettingDict.Add(SoundType.Bgm, new SoundSettings(false, 1));
        soundSettingDict.Add(SoundType.Sfx, new SoundSettings(false, 1));

        //FADE IN OUT을 위한 2개 생성
        for (int i = 0; i < 2; i++)
        {
            bgmAudioSources.Add(CreateAudioSource(SoundType.Bgm.ToString(), true));
        }

        //SFX는 Pitch 조절이 가능해야하니 풀링 가능하게
        sfxAudioSources = new ListableObjectPool<AudioSource>(CreateAudioSource(SoundType.Sfx.ToString()));
        sfxAudioSources.AddAction(ObjectPoolActionType.Instantiate, (audioSource) =>
            {
                audioSource.FixedUpdateAsObservable()
                    .Select(_ => audioSource.isPlaying)
                    .DistinctUntilChanged()
                    .Subscribe(_ =>
                    {
                        if (!audioSource.isPlaying)
                            sfxAudioSources.PushPool(audioSource);
                    });
            })
            .CreatePoolObject(2);
    }

    private AudioSource CreateAudioSource(string audioSourceName, bool isLoop = false)
    {
        var audioSourceObj = new GameObject(audioSourceName);
        audioSourceObj.transform.SetParent(transform);

        var audioSource = audioSourceObj.AddComponent<AudioSource>();
        audioSource.loop = isLoop;

        return audioSource;
    }

    protected override void OnReset()
    {
        StopAllSounds();
    }

    public void StopAllSounds()
    {
        for (var type = SoundType.Bgm; type < SoundType.Max; type++)
            StopSounds(type);
    }

    public void StopSounds(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Bgm:
                foreach (var audioSource in bgmAudioSources)
                    audioSource.Stop();
                break;
            case SoundType.Sfx:
                foreach (var audioSource in sfxAudioSources)
                    audioSource.Stop();
                break;
        }
    }

    public void UpdateVolume(SoundType soundType, SoundSettings setting)
    {
        soundSettingDict[soundType] = setting;

        switch (soundType)
        {
            case SoundType.Bgm:
                foreach (var audioSource in bgmAudioSources)
                    audioSource.volume = setting.Volume;
                break;
            case SoundType.Sfx:
                foreach (var audioSource in sfxAudioSources)
                    audioSource.volume = setting.Volume;
                break;
        }
    }

    public void PlaySoundSfx(string soundName, float pitch = 1)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            StopSounds(SoundType.Sfx);
            return;
        }

        var audioClip = DataManager.Instance.GetAudioClip(soundName);
        var audioInfo = soundSettingDict[SoundType.Sfx];

        var audioSource = sfxAudioSources.PopPool();
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip, audioInfo.Volume);
    }

    public void PlaySoundBgm(string soundName, bool isFade = true)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            StopSounds(SoundType.Bgm);
            return;
        }

        var audioClip = DataManager.Instance.GetAudioClip(soundName);
        var audioInfo = soundSettingDict[SoundType.Bgm];

        var audioSource = bgmAudioSources[bgmPlayIndex];
        bgmPlayIndex = (bgmPlayIndex + 1) % 2;

        var fadeAudioSource = bgmAudioSources[bgmPlayIndex];

        fadeAudioSource.clip = audioClip;
        fadeAudioSource.volume = 0;
        fadeAudioSource.time = 0;
        fadeAudioSource.Play();

        if (isFade)
        {
            BgmSoundFade(audioSource, 0).Forget();
            BgmSoundFade(fadeAudioSource, audioInfo.Volume).Forget();
        }
        else
        {
            audioSource.Stop();
            fadeAudioSource.volume = audioInfo.Volume;
        }
    }

    private async UniTask BgmSoundFade(AudioSource audioSource, float audioVolume, float duration = 1)
    {
        var time = 0f;
        var startVolume = audioSource.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, audioVolume, time / duration);
            await UniTask.Yield();
        }

        audioSource.volume = audioVolume;
        if (audioSource.volume <= 0)
            audioSource.Stop();
    }
}