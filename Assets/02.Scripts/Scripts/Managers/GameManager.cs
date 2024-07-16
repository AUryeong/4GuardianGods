using InGame.Unit;
using UnityEngine;

namespace InGame
{
    public class GameManager : SingletonBehavior<GameManager>
    {
        public PlayerUnit playerUnit;

        private void Start()
        {
            SoundManager.Instance.PlaySoundBgm("Bgm", 0.3f);
        }
    }
}