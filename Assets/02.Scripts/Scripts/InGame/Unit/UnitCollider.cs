using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame.Unit
{
    public class UnitCollider : MonoBehaviour
    {
        [SerializeField] private Collider2D unitCollider;
        public Collider2D Collider
        {
            get
            {
                if (!unitCollider)
                    unitCollider = GetComponent<Collider2D>();
                return unitCollider;
            }
        }

        [SerializeField] private UnitType ownerUnitType;
        private ContactFilter2D contactFilter;
        
        private readonly List<Collider2D> colliders = new();
        private readonly List<Collider2D> prevColliders = new();
        private Action<List<Collider2D>> colliderAction;

        private void Awake()
        {
            contactFilter = new ContactFilter2D().NoFilter();
            switch (ownerUnitType)
            {
                case UnitType.Enemy:
                    contactFilter.SetLayerMask(LayerMask.GetMask("Player", "Brush"));
                    break;
                default:
                case UnitType.None:
                case UnitType.Player:
                    contactFilter.SetLayerMask(LayerMask.GetMask("Enemy", "Boss"));
                    break;
            }
        }

        private void OnEnable()
        {
            ClearColliders();
        }

        public void SetColliderAction(Action<List<Collider2D>> action)
        {
            colliderAction = action;
        }

        public void ClearColliders()
        {
            prevColliders.Clear();
        }

        public void UpdateCollider()
        {
            if (!gameObject.activeInHierarchy)
                return;

            int colliderCount = Collider.OverlapCollider(contactFilter, colliders);
            if (colliderCount <= 0)
                return;
            
            colliders.RemoveAll(x => prevColliders.Contains(x));
            if (colliders.Count <= 0)
                return;
            
            prevColliders.AddRange(colliders);
            colliderAction?.Invoke(colliders);
        }
    }
}