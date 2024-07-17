using InGame;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public class BossMap : Map
    {
        [FormerlySerializedAs("tilemapDefense")]
        [SerializeField] private GameObject tileMapDefense;

        public override void OnEnter()
        {
            base.OnEnter();
            tileMapDefense.gameObject.SetActive(true);
            CameraManager.Instance.SetOrthographic(7);
        }
    }
}