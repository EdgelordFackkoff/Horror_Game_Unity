using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris_Objective : MonoBehaviour
{
    [Header("Objective Information")]
    public Level3ObjectiveManager objective_manager;
    public int objective_number;

    private void OnTriggerEnter(Collider other)
    {
        objective_manager.UpdateObjectives(objective_number);
        Destroy(gameObject);
    }
}
