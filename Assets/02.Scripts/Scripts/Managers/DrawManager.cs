using System;
using System.Collections.Generic;
using DG.Tweening;
using InGame.Unit;
using UnityEngine;

namespace InGame
{
    public class DrawManager : SingletonBehavior<DrawManager>
    {
        public bool IsDrawing => Input.GetMouseButton(0);

        private List<Vector2> vectorList = new();

        [SerializeField] private SpriteRenderer drawBackground;
        [SerializeField] private SpriteRenderer paintUI;
        [SerializeField] private Drawing originDrawing;
        private Drawing drawing;

        public float LeftBrush
        {
            set
            {
                leftBrush = value;
                paintUI.material.SetFloat("_FillAmount", leftBrush / MAX_DISTANCE);
                paintUI.material.SetFloat("_Fade", 1);
                leftBrushShowDuration = UI_REMOVE_DURATION;
            }
            get => leftBrush;
        }
        private float leftBrush;

        private float power;

        private float leftBrushAddDuration = 0;
        private float leftBrushShowDuration = 0;

        private const float BRUSH_ADD_DURATION = 1f;
        private const float UI_REMOVE_DURATION = 1.5f;

        private const float MAX_DISTANCE = 20;
        private const float MIN_DISTANCE = 0.1f;

        private void Update()
        {
            CheckDraw();
        }

        private void LateUpdate()
        {
            var playerPos = GameManager.Instance.playerUnit.transform.position;
            paintUI.transform.position = playerPos;
            drawBackground.transform.position = playerPos;
        }

        public void SetMaxBrush()
        {
            LeftBrush = MAX_DISTANCE;
        }

        private void CheckDraw()
        {
            if (!GameManager.Instance.playerUnit.IsControllable)
            {
                UpdateUI();
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Time.timeScale = 1f;
                drawBackground.DOFade(0, 0.5f).SetUpdate(true);

                drawing.Type = DrawingType.Draw;
                drawing.SetPower(power);

                leftBrushAddDuration = BRUSH_ADD_DURATION;
            }

            if (!IsDrawing)
            {
                UpdateUI();
                return;
            }


            Vector2 input = CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 0.25f;
                drawBackground.DOFade(0.4f, 0.5f).SetUpdate(true);
                paintUI.gameObject.SetActive(true);

                vectorList = new List<Vector2>
                {
                    input,
                    input
                };

                power = 0;

                drawing = Instantiate(originDrawing, Vector3.zero, Quaternion.identity);
                drawing.Type = DrawingType.Drawing;
                drawing.SetPosition(vectorList);
            }

            if (LeftBrush <= 0) return;
            if (Vector2.Distance(vectorList[^1], input) > MIN_DISTANCE)
            {
                float distance = Vector2.Distance(vectorList[^1], input);
                LeftBrush -= distance;
                power += distance;
                vectorList.Add(input);

                drawing.SetPosition(vectorList);
            }
        }

        private void UpdateUI()
        {
            if (leftBrushAddDuration > 0)
            {
                leftBrushAddDuration -= Time.deltaTime;
                return;
            }

            if (LeftBrush < MAX_DISTANCE)
            {
                LeftBrush = Mathf.Clamp(LeftBrush + Time.deltaTime * MAX_DISTANCE, 0, MAX_DISTANCE);
            }

            if (leftBrushShowDuration > 0)
            {
                leftBrushShowDuration -= Time.deltaTime;
                paintUI.material.SetFloat("_Fade", Mathf.Lerp(0, 1, leftBrushShowDuration * 2 / UI_REMOVE_DURATION));
                if (leftBrushShowDuration <= 0)
                {
                    paintUI.gameObject.SetActive(false);
                }
            }
        }
    }
}