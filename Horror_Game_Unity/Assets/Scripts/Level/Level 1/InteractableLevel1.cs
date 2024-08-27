using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLevel1 : Interactable
{
    [Header("Objective Information")]
    public ObjectiveManager objective_manager;
    public int objective_number;

    public override void effect()
    {
        objective_manager.UpdateObjectives(objective_number);
        Destroy(gameObject);
    }
}
