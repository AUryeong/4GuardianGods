using InGame;
using InGame.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public enum EnemyType
    {
        Arm,
        Bird,
        Cow,
        Dog,
        Flower,
        Human,
        Max
    }
    public class BossMap : Map
    {
        [SerializeField] private Boss boss;
        [SerializeField] private GameObject tileMapBackground;
        [FormerlySerializedAs("tilemapDefense")]
        [SerializeField] private GameObject tileMapDefense;

        [SerializeField] private Dictionary<EnemyType, Enemy> enemyDict;

        public override void OnEnter()
        {
            base.OnEnter();
            StartCoroutine(PlayEnterAnimation());
        }

        public override void OnUpdate()
        {
            boss.OnUpdate();
        }

        public void CreateEnemy(EnemyType type, Vector3 position)
        {
            var enemy = Instantiate(enemyDict[type], transform);
            enemy.transform.position = position;
            enemy.OnEnter();
            enemy.gameObject.SetActive(true);
            enemies.Add(enemy);
        }

        private IEnumerator PlayEnterAnimation()
        {
            tileMapDefense.gameObject.SetActive(true);
            tileMapBackground.gameObject.SetActive(true);

            boss.IsControllable = false;

            GameManager.Instance.playerUnit.IsControllable = false;
            CameraManager.Instance.EnterBossRoom();

            yield return new WaitForSeconds(5);

            boss.OnEnter();
            CameraManager.Instance.Shake(1.3f, 10, 3);
            yield return new WaitForSeconds(1);
            
            SoundManager.Instance.PlaySoundBgm("BossBgm");
            yield return new WaitForSeconds(1);
            
            boss.IsControllable = true;
            boss.StartAttack();
            GameManager.Instance.playerUnit.IsControllable = true;
        }
    }
}