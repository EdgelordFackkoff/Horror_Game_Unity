using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Collision_Activation_First : MonoBehaviour
{
    public Level level;

    void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("Collision Detected");
        // Check if player
        if (other.gameObject.layer == 3)
        {
            UnityEngine.Debug.Log("Player Col Detected");
            //Activate the initials
            level.activate_initial_knights();

            //Destroy itself
            Destroy(gameObject);
        }
    }
}
