using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pressure_Plate_set : MonoBehaviour
{
    [Header("Unique")]
    [SerializeField] public Level level;
    [SerializeField] private Pressure_Plate[] plates;
    [SerializeField] private Doors[] affected_doors;
    [SerializeField] private bool effectTriggered = false;
    [SerializeField] private bool activate_section_knights;
    [SerializeField] private int activated_section_knights;
    [SerializeField] private float increase_exposure_amount;

    void Start()
    {
        //Reference Level
        level = GetComponentInParent<Level>();
        // Get all the PressurePlate components from children
        plates = GetComponentsInChildren<Pressure_Plate>();
    }

    void Update()
    {
        if (!effectTriggered && AllPlatesActivated())
        {
            effectTriggered = true;
            TriggerEffect();
            DisableAllPlates();
        }
    }

    private bool AllPlatesActivated()
    {
        foreach (var plate in plates)
        {
            if (!plate.is_activated)
                return false;
        }
        return true;
    }

    private void TriggerEffect()
    {
        //Doors
        foreach (Doors obj in affected_doors)
        {
            if (obj != null)
            {
                obj.OpenDoor();
            }
        }

        //Section knights
        if (activate_section_knights)
        {
            level.activate_section_knights(activated_section_knights);
        }
    }

    private void DisableAllPlates()
    {
        foreach (var plate in plates)
        {
            increase_exposure();
            plate.jobs_done();
        }
    }

    public void increase_exposure()
    {
        if (increase_exposure_amount > 0)
        {
            level.increase_exposure_amount(increase_exposure_amount);
        }
    }
}
