using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InGame
{
    public class UIManager : SingletonBehavior<UIManager>
    {
        [SerializeField] private SpriteRenderer sceneBlackSpriteRenderer;
        [SerializeField] private SpriteRenderer hitSpriteRenderer;
        private Tweener tweener;

        protected override void OnCreated()
        {
            base.OnCreated();
            hitSpriteRenderer.color = Color.red;
        }

        private void Start()
        {
            sceneBlackSpriteRenderer.DOFade(0, 2).OnComplete(() => sceneBlackSpriteRenderer.gameObject.SetActive(false));
        }

        public void BossDie()
        {
            sceneBlackSpriteRenderer.transform.position = GameManager.Instance.playerUnit.transform.position;
            sceneBlackSpriteRenderer.gameObject.SetActive(true);
            sceneBlackSpriteRenderer.DOFade(1, 2).OnComplete(() =>
            {
                SceneManager.LoadScene("Title");
            });
        }

        public void Hit()
        {
            CameraManager.Instance.Shake(0.5f, 2.5f, 10);

            SoundManager.Instance.PlaySoundSfx("Hurt2", 1, 1.5f);
            SoundManager.Instance.PlaySoundAmbient("Hurt", 0.1f, 2f);

            hitSpriteRenderer.transform.position = GameManager.Instance.playerUnit.transform.position;

            hitSpriteRenderer.gameObject.SetActive(true);
            hitSpriteRenderer.color = hitSpriteRenderer.color.GetChangeAlpha(0.5f);
            hitSpriteRenderer.DOFade(0, 0.5f).SetEase(Ease.InQuad).SetUpdate(true).OnComplete(() => { hitSpriteRenderer.gameObject.SetActive(false); });

            Time.timeScale = 0.2f;
            tweener = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 1f).SetEase(Ease.InQuad).SetUpdate(true).OnComplete(() => { SoundManager.Instance.StopSoundAmbient("Hurt"); });
        }

        public void Die()
        {
            if (Time.timeScale <= 0)
                return;

            tweener?.Kill();
            CameraManager.Instance.Shake(1, 5, 10f);

            var sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.AppendCallback(() =>
            {
                Time.timeScale = 0;
                SoundManager.Instance.StopSounds(SoundType.Bgm);
                SoundManager.Instance.StopSounds(SoundType.Ambient);

                hitSpriteRenderer.DOKill();
                hitSpriteRenderer.color = hitSpriteRenderer.color.GetChangeAlpha(0.8f);
            });
            sequence.Append(hitSpriteRenderer.DOFade(0, 0.2f).SetEase(Ease.Linear).SetUpdate(true).SetLoops(5, LoopType.Yoyo));
            sequence.AppendInterval(1.1f);

            sequence.Append(hitSpriteRenderer.DOFade(0.8f, 1));
            sequence.AppendInterval(2f);

            sequence.Append(hitSpriteRenderer.DOColor(Color.black, 1));
            sequence.AppendInterval(1f);

            sequence.AppendCallback(() =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("InGame");
            });
        }
    }
}