using InGame.Unit;
using UnityEngine;

namespace InGame
{
    public class GameManager : SingletonBehavior<GameManager>
    {
        public PlayerUnit playerUnit;
        public GameObject dieEffect;

        private void Start()
        {
            SoundManager.Instance.PlaySoundBgm("Bgm", 0.2f);
            SoundManager.Instance.PlaySoundAmbient("Birds", 1f);
        }
    }
}