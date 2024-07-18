using InGame;
using InGame.Unit;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public class BossMap : Map
    {
        [SerializeField] private Boss boss;
        [FormerlySerializedAs("tilemapDefense")]
        [SerializeField] private GameObject tileMapDefense;

        public override void OnEnter()
        {
            base.OnEnter();
            StartCoroutine(PlayEnterAnimation());
        }

        public override void OnUpdate()
        {
            boss.OnUpdate();
        }

        private IEnumerator PlayEnterAnimation()
        {
            tileMapDefense.gameObject.SetActive(true);

            boss.IsControllbale = false;

            GameManager.Instance.playerUnit.IsControllable = false;
            CameraManager.Instance.EnterBossRoom();

            yield return new WaitForSeconds(5);
            
            boss.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);

            boss.UnitAnimator.PlayAnimationClip(UnitAnimationType.Special);
            CameraManager.Instance.Shake(1.3f, 5, 10);
            
            yield return new WaitForSeconds(2);
            
            boss.IsControllbale = true;
            boss.StartAttack();
            GameManager.Instance.playerUnit.IsControllable = true;
        }
    }
}