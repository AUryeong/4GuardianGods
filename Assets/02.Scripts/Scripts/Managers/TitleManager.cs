using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : SingletonBehavior<TitleManager>
{
    [SerializeField] private Image blackBackground;

    [SerializeField] private TitleButton startButton;
    [SerializeField] private TitleButton exitButton;
    protected override void OnCreated()
    {
        base.OnCreated();
        startButton.button.onClick.AddListener(() =>
        {
            blackBackground.gameObject.SetActive(true);
            blackBackground.DOFade(1, 2).OnComplete(() => SceneManager.LoadScene("InGame"));
        });
        exitButton.button.onClick.AddListener(() => Application.Quit());
    }

    private void Start()
    {
        SoundManager.Instance.PlaySoundAmbient("Birds", 1f);
    }
}
