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
                    contactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
                    break;
            }
        }

        public void SetColliderAction(Action<List<Collider2D>> action)
        {
            colliderAction = action;
        }

        public void UpdateCollider(float deltaTime)
        {
            int colliderCount = Collider.OverlapCollider(contactFilter, colliders);
            if (colliderCount <= 0)
                return;
            
            colliderAction?.Invoke(colliders);
        }
    }
}