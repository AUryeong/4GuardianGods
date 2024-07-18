using System.Collections;
using UnityEngine;

namespace InGame
{
    public class BossEnterMap : Map
    {
        public override void OnEnter()
        {
            base.OnEnter();
            SoundManager.Instance.PlaySoundAmbient("Fire", 0.4f);
            SoundManager.Instance.StopSounds(SoundType.Bgm);
        }
    }
}