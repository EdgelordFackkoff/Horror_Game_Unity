using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Damagebox : MonoBehaviour
{
    //Reference enemy script
    [SerializeField] private Enemy attached_enemy;

    // Grab enemy
    public Enemy get_enemy()
    {
        return attached_enemy;
    }
}
