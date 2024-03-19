using InGame.Unit;
using UnityEngine;

namespace InGame
{
    public class GameManager : SingletonBehavior<GameManager>
    {
        [SerializeField] private PlayerUnit playerUnit;
    }
}