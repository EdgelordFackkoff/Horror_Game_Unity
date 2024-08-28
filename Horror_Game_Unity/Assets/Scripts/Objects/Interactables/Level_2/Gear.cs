using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Gear : Interactable
{
    [Header("Unique")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private bool activate_section_knights = false;
    [SerializeField] private int activated_section_knights;
    [SerializeField] private AudioSource gear_audio;

    public override void effect()
    {
        //Remove interactability
        set_interactability(0);

        //Usual
        increase_exposure();

        //Open door
        foreach (GameObject obj in affected_objects)
        {
            if (obj != null)
            {
                Doors door = obj.GetComponent<Doors>();
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }

        //Audio
        gear_audio.Play();

        //Section knights
        if (activate_section_knights)
        {
            level.activate_section_knights(activated_section_knights);
        }
    } 
}
