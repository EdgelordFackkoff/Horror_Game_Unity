using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Game State")]
    //Initially false
    public bool paused = false;

    [Header("Exposure")]
    public int exposure_level = 0;
    public int exposure_level_max = 3;
    public float exposure_amount = 0;
    public float exposure_amount_max = 100.0f;
    //For data reference
    private int last_exposure_level = 0;

    [Header("Input")]
    public float mouse_sensitivity = 2.0f;
    public string horizontal_move_input = "Horizontal";
    public string vertical_move_input = "Vertical";
    public KeyCode sprint_key = KeyCode.LeftShift;
    public KeyCode jump_key = KeyCode.Space;
    public KeyCode interact_key = KeyCode.E;

    [Header("Level Audio")]
    public AudioSource player_music_source;
    public AudioSource player_exposure_alert_source;
    [SerializeField] private AudioClip level_exposure_increase_alert;
    [SerializeField] private AudioClip level_exposure_decrease_alert;
    [SerializeField] private AudioClip[] level_music;
    [SerializeField] private int current_music;

    [Header("Global/Shared Variables")]
    public Player player;
    public float gravity = 9.18f;

    //Start
    void Awake()
    {
        //No exposure yet
        exposure_level = 0;
        exposure_amount = 0;

        //Lock Cursor
        LockCursor();

        //Play it
        ChangeMusic(0);
    }

    void Update()
    {
        HandleExposureChange();
    }

    //Lock Cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Unlock Cursor
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        //Later code here for other things
    }

    //Pause
    public void game_pause()
    {
        //later
    }

    //Unpause
    public void game_unpause()
    {
        //Later
    }

    //Return player
    public Player return_player()
    {
        return player;
    }

    //MUSIC
    private void ChangeMusic(int x)
    {
        if (level_music.Length == 1)
        {
            player_music_source.clip = level_music[0];
        }
        else
        {
            if (x < level_music.Length)
            {
                player_music_source.clip = level_music[x];
            }
            
        }

        //Change current music
        current_music = x;
        if (player_music_source != null)
        {
            player_music_source.Play();
        }
    }

    //EXPOSURE

    private void HandleExposureChange()
    {
        //Detect if change
        if (exposure_level != last_exposure_level)
        {
            //Do stuff here maybe

            //Music change
            ChangeMusic(exposure_level);

            //Play the alert clip
            if (last_exposure_level < exposure_level)
            {
                //Exposure Increase Alert
                player_exposure_alert_source.clip = level_exposure_increase_alert;
                player_exposure_alert_source.Play();
            }
            else
            {
                //Exposure Decrease Alert
                player_exposure_alert_source.clip = level_exposure_decrease_alert;
                player_exposure_alert_source.Play();
            }

            //New level
            last_exposure_level = exposure_level;
        }
    }

    public void increase_exposure_amount(float amount)
    {
        float check_amount = exposure_amount + amount;

        //Handle increase
        while (check_amount >= 100.0f)
        {
            if (exposure_level < 3)
            {
                increase_exposure_level(1);
                check_amount -= 100.0f;
            }

            //Cap it
            else
            {
                //Workaround
                check_amount = 99.9f;
                break;
            }
        }

        //Workaround
        exposure_amount = check_amount + 0.1f;
    }

    public void increase_exposure_level(int increase)
    {
        //Detect if not 4 and if added will not go over 4
        if (exposure_level < 3 && increase + exposure_level <= 3)
        {
            //Add
            exposure_level += increase;
        }
        //Else set to 3
        else
        {
            exposure_level = 3;
        }

        UnityEngine.Debug.Log("Current Exposure Level: " + exposure_level);
    }

    public void decrease_exposure_amount(float amount)
    {
        //Make it negative
        amount = -1.0f * amount;
        float check_amount = exposure_amount + amount;

        //Handle increase
        while (check_amount < 0.0f)
        {
            if (exposure_level > 0)
            {
                decrease_exposure_level(1);
                check_amount += 100.0f;
            }

            //Cap it
            else
            {
                //Workaround
                check_amount = 0.1f;
                break;
            }
        }

        //Workaround
        exposure_amount = check_amount - 0.1f;
    }

    public void decrease_exposure_level(int decrease)
    {
        //Detect if not 0 and if decreased will not go under 0
        if (exposure_level > 0 && exposure_level - decrease >= 0)
        {
            //Add
            exposure_level -= decrease;
        }
        //Else set to 0
        else
        {
            exposure_level = 0;
        }
    }
}
