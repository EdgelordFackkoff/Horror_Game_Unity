using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Objective : MonoBehaviour
{
    [Header("Objective Information")]
    public Level3ObjectiveManager objective_manager;
    public int objective_number;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        if (objective_manager != null)
        {
            //Will rewrite with interact
            objective_manager.UpdateObjectives(objective_number);
        }
    }
}
