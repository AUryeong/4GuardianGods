using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    [SerializeField] private TextMeshProUGUI text;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.color = Color.black;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.Play("End");
        text.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.Play("Start");
        text.color = Color.black;
    }
}
