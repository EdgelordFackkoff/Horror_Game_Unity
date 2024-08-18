using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Hitbox : Player_Hitbox
{
    // Override
    //From inheritance
    public override void effect_enter(Collider other)
    {
        //Grab enemy components
        Enemy_Damagebox damage_box = other.GetComponent<Enemy_Damagebox>();
        Enemy enemy = damage_box.get_enemy();
        UnityEngine.Debug.Log("Damage collision");
        //If enemy isn't null
        if (enemy != null)
        {
            //Check if it can attack
            if (enemy.can_attack == true)
            {
                //Run damage effect for player
                player.Attacked(enemy);
            }
        }
    }

    public override void effect_stay(Collider other)
    {
        //Grab enemy components
        Enemy_Damagebox damage_box = other.GetComponent<Enemy_Damagebox>();
        Enemy enemy = damage_box.get_enemy();
        //If enemy isn't null
        if (enemy != null)
        {
            //Check if it can attack
            if (enemy.can_attack == true)
            {
                //Run damage effect for player
                player.Attacked(enemy);
            }
        }
    }
    public override void effect_exit(Collider other)
    {
        //Nothing really
    }
}
