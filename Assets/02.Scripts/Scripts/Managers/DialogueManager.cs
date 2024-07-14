using System.Collections;
using InGame;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Febucci.UI;

public class DialogueManager : SingletonBehavior<DialogueManager>
{
    private const string REMAINDER_REGEX = "(.*?((?=})|(/|$)))";
    private const string COMMAND_REGEX_STRING = "{(?<command>" + REMAINDER_REGEX + ")}";
    private static readonly Regex COMMAND_REGEX = new Regex(COMMAND_REGEX_STRING);
    private const float NEXT_COOLTIME = 0.05f;

    private bool isTexting = false;
    private int idx;
    private float nextDuration;

    [SerializeField] private Image dialogueImage;
    [SerializeField] private Image dialoguePopup;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private void Update()
    {
        dialogueImage.rectTransform.position = Camera.main.WorldToScreenPoint(GameManager.Instance.playerUnit.transform.position) + new Vector3(0, 90);
        if (Input.GetKeyDown(KeyCode.F))
        {
            dialogueImage.gameObject.SetActive(true);
            ConvertText("{bbs}아싸!!!{/bbs} 이제부터 난 {bw}대장님{/bw}이다!");
            isTexting = true;
            idx = 0;
            nextDuration = 0;

            dialogueImage.DOKill();
            dialogueImage.color = dialogueImage.color.GetChangeAlpha(1);

            dialoguePopup.DOKill();
            dialoguePopup.color = dialoguePopup.color.GetChangeAlpha(1);

            dialogueText.DOKill();
            dialogueText.color = dialogueText.color.GetChangeAlpha(1);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            dialogueText.GetComponent<TypewriterByCharacter>().StartDisappearingText();
            dialoguePopup.DOFade(0, 0.5f);
            dialogueImage.DOFade(0, 0.5f).OnComplete(() =>
            {
                dialogueImage.gameObject.SetActive(false);
            });
        }

        if (!isTexting) return;

        if (idx >= dialogueText.textInfo.characterCount - 1)
        {
            isTexting = false;

            dialogueImage.DOFade(0, 1).SetDelay(1);
            dialoguePopup.DOFade(0, 1).SetDelay(1);
            dialogueText.DOFade(0, 1).SetDelay(1).OnComplete(() =>
            {
                dialogueImage.gameObject.SetActive(false);
            });
            return;
        }
        nextDuration += Time.deltaTime;
        if (nextDuration >= NEXT_COOLTIME)
        {
            nextDuration -= NEXT_COOLTIME;
            idx++;
        }
        dialogueText.maxVisibleCharacters = idx + 1;
        dialogueText.ForceMeshUpdate();
    }

    private void ConvertText(string text)
    {
        float boldMultipler = 1f;
        int indexAdd = 0;
        foreach (Match match in COMMAND_REGEX.Matches(text))
        {
            string command = match.Groups["command"].Value;
            bool isRemove = false;

            foreach (char c in command)
            {
                switch (c)
                {
                    case '/':
                        isRemove = true;
                        break;
                    case 'b':
                        boldMultipler = isRemove ? boldMultipler / 1.4f : boldMultipler * 1.4f;
                        break;
                    case 's':
                        string shakeText = isRemove ? "</shake>" : "<shake>";
                        text = text.Insert(match.Index + indexAdd, shakeText);
                        indexAdd += shakeText.Length;
                        break;
                    case 'w':
                        string waveText = isRemove ? "</wave>" : "<wave>";
                        text = text.Insert(match.Index + indexAdd, waveText);
                        indexAdd += waveText.Length;
                        break;
                }
            }

            string sizeText = "<size=" + Mathf.CeilToInt(boldMultipler * 100) + "%>";
            text = text.Insert(match.Index + indexAdd, sizeText);
            indexAdd += sizeText.Length;
        }
        text = Regex.Replace(text, COMMAND_REGEX_STRING, "");
        dialogueText.text = text;
    }
}
