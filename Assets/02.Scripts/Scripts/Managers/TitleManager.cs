using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : SingletonBehavior<TitleManager>
{
    [SerializeField] private TitleButton startButton;
    [SerializeField] private TitleButton exitButton;
    protected override void OnCreated()
    {
        base.OnCreated();
        startButton.button.onClick.AddListener(() => SceneManager.LoadScene("InGame"));
        exitButton.button.onClick.AddListener(() => Application.Quit());
    }

    private void Start()
    {
        SoundManager.Instance.PlaySoundAmbient("Birds", 1f);
    }
}
