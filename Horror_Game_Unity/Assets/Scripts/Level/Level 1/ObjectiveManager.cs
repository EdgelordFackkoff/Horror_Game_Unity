using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Level")]
    public Level level;

    [Header("Audio")]
    public AudioSource collection_sound;
    public AudioSource exit_sound;


    [Header("UI")]
    public TextMeshProUGUI objective_text;

    [Header("Objectives")]
    public List<bool> objectives;
    int completed_objectives_count;
    public bool can_exit;

    public void UpdateObjectives(int objective_index)
    {
        getCompleted();
        objectives[objective_index] = true;

        level.increase_exposure_amount(25);
        updateExit();
        if (!can_exit)
        {
            collection_sound.Play();
            objective_text.text = completed_objectives_count + " / 15 books";
        }
        else
        {
            exit_sound.Play();
            objective_text.text = "Find the hidden exit!";
        }
    }

    public void updateExit()
    {
        can_exit = objectives.All(value => value);
    }

    public void getCompleted()
    {
        completed_objectives_count = objectives.Count(value => value);
        completed_objectives_count = completed_objectives_count + 1;
    }
}
