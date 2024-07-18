using System.Collections;
using UnityEngine;

namespace InGame
{
    public class BossEnterMap : Map
    {
        public override void OnEnter()
        {
            base.OnEnter();
            SoundManager.Instance.StopSounds(SoundType.Bgm);
        }
    }
}