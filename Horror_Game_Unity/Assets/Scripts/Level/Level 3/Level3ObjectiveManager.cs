using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Level3ObjectiveManager : MonoBehaviour
{
    [Header("Objectives")]
    public List<bool> objectives;

    public void UpdateObjectives(int objective_index)
    {
        objectives[objective_index] = true;
    }
}
