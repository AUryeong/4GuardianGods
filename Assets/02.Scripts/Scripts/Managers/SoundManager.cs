using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public enum SoundType
{
    None = -1,
    Bgm,
    Sfx,
    Ambient,
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
    protected override bool IsDontDestroying => true;
    private readonly Dictionary<SoundType, SoundSettings> soundSettingDict = new((int)SoundType.Max);
    private readonly List<AudioSource> bgmAudioSources = new(2);
    private int bgmPlayIndex;
    private ListableObjectPool<AudioSource> sfxAudioSources;
    private Dictionary<string, AudioSource> ambientAudioSourceDict;

    protected override void OnCreated()
    {
        base.OnCreated();

        //TODO 설정 적용
        for (SoundType soundType = SoundType.None+1; soundType < SoundType.Max; soundType++)
        {
            soundSettingDict.Add(soundType, new SoundSettings(false, 1));
        }

        //FADE IN OUT을 위한 2개 생성
        for (int i = 0; i < 2; i++)
        {
            bgmAudioSources.Add(CreateAudioSource(SoundType.Bgm.ToString(), true));
        }

        //AMBIENT는 크고 껴고가 잦으며, 특정 키를 기반으로 키고 끄게
        ambientAudioSourceDict = new Dictionary<string, AudioSource>();

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
        for (var type = SoundType.None + 1; type < SoundType.Max; type++)
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
            case SoundType.Ambient:
                foreach (var audioSource in ambientAudioSourceDict.Values)
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
            case SoundType.Ambient:
                foreach (var audioSource in ambientAudioSourceDict.Values)
                    audioSource.volume = setting.Volume;
                break;
        }
    }

    public void StopSoundAmbient(string soundKey)
    {
        if (ambientAudioSourceDict.TryGetValue(soundKey, out var audioSource))
        {
            audioSource.Stop();
        }
    }

    public void PlaySoundAmbient(string soundKey, float volume = 1, float pitch = 1)
    {
        if (string.IsNullOrEmpty(soundKey))
        {
            StopSounds(SoundType.Ambient);
            return;
        }

        var audioInfo = soundSettingDict[SoundType.Ambient];
        if (ambientAudioSourceDict.TryGetValue(soundKey, out var audioSource))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.time = 0;
                audioSource.Play();
            }
        }
        else
        {
            audioSource = CreateAudioSource(soundKey, true);

            var audioClip = DataManager.Instance.GetAudioClip(soundKey);
            audioSource.pitch = pitch;
            audioSource.volume = audioInfo.Volume * volume;
            audioSource.clip = audioClip;
            audioSource.Play();
            ambientAudioSourceDict.Add(soundKey, audioSource);
        }
    }

    public void PlaySoundSfx(string soundName, float volume = 1, float pitch = 1)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            StopSounds(SoundType.Sfx);
            return;
        }

        var audioClip = DataManager.Instance.GetAudioClip(soundName);
        var audioInfo = soundSettingDict[SoundType.Sfx];

        var audioSource = CreateAudioSource(SoundType.Sfx.ToString());
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip, audioInfo.Volume* volume);
    }

    public void PlaySoundBgm(string soundName, float volume = 1, bool isFade = true)
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
            BgmSoundFade(fadeAudioSource, audioInfo.Volume* volume).Forget();
        }
        else
        {
            audioSource.Stop();
            fadeAudioSource.volume = audioInfo.Volume* volume;
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