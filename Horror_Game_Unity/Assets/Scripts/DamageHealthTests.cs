using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class DamageHealthTests : MonoBehaviour
{
    [SerializeField] private int Type;

    //Get collider
    void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("Collided");

        //If player
        if (collision.gameObject.layer == 3)
        {
            UnityEngine.Debug.Log("Player Detected}");

            //Get player script
            Player player = collision.gameObject.GetComponentInParent<Player>();

            //Heal or Damage
            if (Type == 1)
            {
                //Damage
                player.TakeDamage(25.0f);
            }

            //Heal
            if (Type == 2)
            {
                //Damage
                player.HealDamage(25.0f);
            }
        }
    }
}
