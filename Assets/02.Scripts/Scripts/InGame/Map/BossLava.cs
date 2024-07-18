using InGame.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    public class BossLava : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<UnitHit>().Hit(1);
            }
        }
    }
}