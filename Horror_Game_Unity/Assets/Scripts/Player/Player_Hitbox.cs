using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player_Hitbox : MonoBehaviour
{
    [Header("Info")]
    //1 or 0. 1 is damage, 2 is interact, 3 is pickup
    [SerializeField] private int type;
    [SerializeField] public Player player;
    [SerializeField] private int active_layer;

    //For inheritance
    public abstract void effect_enter(Collider other);
    public abstract void effect_stay(Collider other);
    public abstract void effect_exit(Collider other);

        void Awake()
        {
            //Grab player
            player = GetComponentInParent<Player>();
        }

        //See if colliding with layer active
        void onTriggerEnter(Collider other)
        {
         bool x = checkLayer(other);
        if (x)
            {
                effect_enter(other);
            }
        }

        void OnTriggerStay(Collider other)
        {

            bool x = checkLayer(other);
            if (x)
            {
                effect_stay(other);
            }
        }

        void OnTriggerExit(Collider other)
        {

            bool x = checkLayer(other);
            if (x)
            {
                effect_exit(other);
            }
        }

        //For checking layer
        private bool checkLayer(Collider other)
        {
        
        bool x = false;

                if (other.gameObject.layer == active_layer)
                    {
                        x = true;
                    }
        
        return x;
        }
    }

