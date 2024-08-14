using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Hitbox : Player_Hitbox
{
    //From inheritance
    public override void effect_enter(Collider other)
    {
        //Grab interactable component
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable.canInteract() == true && interactable != null)
        {
            //Get info to UI via player
            player.HandleInteractableEnter(other);
        }

        //Set to null
        else
        {
            player.HandleInteractableNone();
        }

    }

    public override void effect_stay(Collider other)
    {

        //Grab interactable component
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable.canInteract() == true && interactable != null)
        {
            //Get info to UI via player
            player.HandleInteractableStay(other);
        }

        //Set to null
        else
        {
            player.HandleInteractableNone();
        }
    }

    public override void effect_exit(Collider other)
    {

        //Grab interactable component
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable.canInteract() == true && interactable != null)
        {
            //Get info to UI via player
            player.HandleInteractableExit(other);
        }

        //Set to null
        else
        {
            player.HandleInteractableNone();
        }
    }
}
