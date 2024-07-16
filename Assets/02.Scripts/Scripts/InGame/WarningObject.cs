using InGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public bool isFollowPlayer;
    [SerializeField] private Transform parent;

    [SerializeField] private bool isUseDuration;
    [SerializeField] private float maxDuration;
    private float duration;

    [SerializeField] private float fadeOutMaxDuration = 0.1f;
    private float fadeOutDuration;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parent ??= transform.parent;
    }

    private void OnEnable()
    {
        Enable();
    }

    public void Enable()
    {
        duration = maxDuration;
        fadeOutDuration = 0;
        spriteRenderer.color = Color.red;
    }

    public void Disable()
    {
        fadeOutDuration = fadeOutMaxDuration;
        spriteRenderer.color = Color.yellow;
    }

    private void Update()
    {
        if (isFollowPlayer)
        {
            var direction = (GameManager.Instance.playerUnit.transform.position - parent.transform.position).normalized;
            var rayCastHit = Physics2D.Raycast(parent.transform.position, direction, float.PositiveInfinity, LayerMask.GetMask("Platform", "Brush"));
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            spriteRenderer.transform.position = ((Vector3)rayCastHit.point + parent.transform.position) / 2;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
            spriteRenderer.size = new Vector2(rayCastHit.distance, spriteRenderer.size.y);
        }
        if (fadeOutDuration > 0)
        {
            fadeOutDuration -= Time.deltaTime;
            if (fadeOutDuration < 0)
            {
                gameObject.SetActive(false);
            }
        }
        if (isUseDuration)
        {
            if (duration > 0)
            {
                duration -= Time.deltaTime;
                if (duration <= 0)
                {
                    Disable();
                }
            }
        }
    }
}
