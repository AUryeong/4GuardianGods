using InGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform parent;

    public bool isFollowPlayer;

    [Header("Duration")]
    [SerializeField] private bool isUseDuration;
    [SerializeField] private float maxDuration;
    private float duration;

    [SerializeField] private float fadeOutMaxDuration = 0.4f;
    private float fadeOutDuration;
    private const int FADE_OUT_FLASH_TIME = 8;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parent ??= transform.parent;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetActive(true);
    }

    public void SetActive(bool _isActive)
    {
        if (_isActive == gameObject.activeSelf)
            return;

        if (_isActive)
        {
            gameObject.SetActive(true);

            duration = maxDuration - fadeOutMaxDuration;
            fadeOutDuration = 0;
            spriteRenderer.color = Color.red.GetChangeAlpha(spriteRenderer.color.a);
        }
        else
        {
            fadeOutDuration = fadeOutMaxDuration;
        }
    }

    private void Update()
    {
        if (fadeOutDuration > 0)
        {
            fadeOutDuration -= Time.deltaTime;
            spriteRenderer.color = fadeOutDuration / (fadeOutMaxDuration / FADE_OUT_FLASH_TIME) % 2 >= 1
                ? Color.red.GetChangeAlpha(spriteRenderer.color.a)
                : Color.white.GetChangeAlpha(spriteRenderer.color.a);

            if (fadeOutDuration < 0)
            {
                gameObject.SetActive(false);
            }

            return;
        }

        if (isFollowPlayer)
        {
            var direction = (GameManager.Instance.playerUnit.transform.position - parent.transform.position).normalized;
            var rayCastHit = Physics2D.Raycast(parent.transform.position, direction, float.PositiveInfinity, LayerMask.GetMask("Platform", "Brush"));
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            spriteRenderer.transform.position = ((Vector3)rayCastHit.point + parent.transform.position) / 2;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
            spriteRenderer.size = new Vector2(rayCastHit.distance, spriteRenderer.size.y);
        }

        if (isUseDuration)
        {
            if (duration > 0)
            {
                duration -= Time.deltaTime;
                if (duration <= 0)
                {
                    SetActive(false);
                }
            }
        }
    }
}