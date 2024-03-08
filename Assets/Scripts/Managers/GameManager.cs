using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonBehavior<GameManager>
{
    private const int TARGET_FRAME_RATE = 60;
    protected override bool IsDontDestroying => true;

    protected override void OnCreated()
    {
        base.OnCreated();

        Application.targetFrameRate = TARGET_FRAME_RATE;
        
        DataManager.Instance.Init();
    }
}