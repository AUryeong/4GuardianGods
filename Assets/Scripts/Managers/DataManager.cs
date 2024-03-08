using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : Singleton<DataManager>
{
    public bool IsLoadingComplete { get; private set; }
    private SheetDataDictionary<SheetData> sheetDataDict;

    private Action sheetAction;

    [Header("Sound")]
    private const string SOUND_PATH = "Sounds/";
    private readonly Dictionary<string, AudioClip> audioClips = new();

    public void Init()
    {
        LoadDataTask().Forget();
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

    private async UniTaskVoid LoadDataTask()
    {
        IsLoadingComplete = false;
        Debug.Log("Loading Start...");

        LoadAudioClips();

        sheetDataDict = await SheetDataDictionary<SheetData>.LoadData("0");

        sheetAction?.Invoke();
        Debug.Log("Loading End!");
        IsLoadingComplete = true;
    }

    public void AddLoadDataAction(Action action)
    {
        if (IsLoadingComplete)
        {
            action?.Invoke();
            return;
        }
        sheetAction += action;
    }
    
    private async UniTask<SheetDataDictionary<T>> LoadData<T>(string sheetName) where T : Data
    {
        return await SheetDataDictionary<T>.LoadData(sheetDataDict[sheetName].sheetID);
    }
}

public class SheetDataDictionary<T> where T : Data
{
    private const string ADDRESS = "https://docs.google.com/spreadsheets/d/1fMcCASP0jthZ0Cl1fmeBrgn4XvvWP8NmOg6FNQaSj00";
    private readonly Dictionary<string, T> dictionary;
    private List<T> dataList;

    private SheetDataDictionary()
    {
        dictionary = new Dictionary<string, T>();
    }

    public T this[string key] => dictionary[key];

    public List<T> GetList()
    {
        return dataList ??= dictionary.Values.ToList();
    }

    public static async UniTask<SheetDataDictionary<T>> LoadData(string dataSheetID)
    {
        var dataDict = new SheetDataDictionary<T>();

        var defaultWww = UnityWebRequest.Get($"{ADDRESS}/export?format=tsv&gid={dataSheetID}");
        await defaultWww.SendWebRequest();

        Debug.Assert(defaultWww.result == UnityWebRequest.Result.Success);

        string sheetData = defaultWww.downloadHandler.text;
        var lines = sheetData.Split('\n');
        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.IsEmptyOrWhiteSpace()) continue;

            var data = Activator.CreateInstance<T>();
            data.Init(line.Split('\t'));

            dataDict.dictionary.Add(data.dataID, data);
        }

        return dataDict;
    }
}

public class Data
{
    public string dataID;

    public virtual void Init(string[] columns)
    {
        dataID = columns[0];
    }
}

public class SheetData : Data
{
    public string sheetID;

    public override void Init(string[] columns)
    {
        base.Init(columns);
        sheetID = columns[1];
    }
}