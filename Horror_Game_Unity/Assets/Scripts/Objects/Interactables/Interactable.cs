using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Info")]
    //Name and Desc
    [SerializeField] private string interactable_name;
    [SerializeField] private string interactable_description;
    //Check if can interact frist
    [SerializeField] private bool can_interact;
    //For doors, secret walls or other things etc
    //Done in this way so one object interacted can affect multiple objects
    [SerializeField] private GameObject[] affected_objects;
    //Variable to detect if it is an instant interact
    [SerializeField] private bool instant_interactable;
    //If there is a period where you have to constantly interact with said object
    [SerializeField] private float interactable_value;

    [Header("References")]
    [SerializeField] public GameObject model_object;
    [SerializeField] public Level level;

    //start
    private void Start()
    {
        //Reference Level
        level = GetComponentInParent<Level>();
    }

    //For inheritance
    public abstract void effect();

    //CHECKS
    public bool canInteract()
    {
        return can_interact;
    }

    public bool instant_interact()
    {
        return instant_interactable;
    }

    public float interact_value()
    {
        return interactable_value;
    }

    public string get_name()
    {
        return interactable_name;
    }

    public string get_desc()
    {
        return interactable_description;
    }

    //Changes
    public void add_value(float value)
    {
        interactable_value += value;
    }

    public void remove_value(float value)
    {
        interactable_value -= value;
    }


    //0 is false, 1 is yes
    public void set_interactability(int x)
    {
        if (x != 0 || x != 1)
        {
            //ERROR not supposed to be here
        }
        else
        {
            switch (x)
            {
                case 0:
                    can_interact = false;break;
                case 1:
                    can_interact = true; break;
                default:
                    //ERROR you shouldn't be here
                    can_interact = false; break;
            }
        }
    }
}
