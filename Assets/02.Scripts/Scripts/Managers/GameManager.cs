using InGame.Unit;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace InGame
{
    public class GameManager : SingletonBehavior<GameManager>
    {
        public PlayerUnit playerUnit;
        public GameObject dieEffect;
        public BossMap bossMap;

        private void Start()
        {
            SoundManager.Instance.PlaySoundBgm("Bgm", 0.2f);
            SoundManager.Instance.PlaySoundAmbient("Birds", 1f);

            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}