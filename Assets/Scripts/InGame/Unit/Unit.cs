using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Unit
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] protected UnitMover unitMover;
        [SerializeField] protected UnitAnimator unitAnimator;

        protected virtual void Awake()
        {
            if (unitMover != null)
                unitMover = GetComponent<UnitMover>();

            if (unitAnimator != null)
                unitAnimator = GetComponent<UnitAnimator>();
        }

        protected virtual void Update()
        {
        }
    }

}