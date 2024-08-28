using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Game State")]
    //Initially false
    public bool paused = false;
    public int level_int;

    [Header("Exposure")]
    public int exposure_level = 0;
    public int exposure_level_max = 3;
    public float exposure_amount = 0;
    public float exposure_amount_max = 100.0f;
    //For data reference
    private int last_exposure_level = 0;

    [Header("Input")]
    [SerializeField] public float mouse_sensitivity = 2.0f;
    [SerializeField] public string horizontal_move_input = "Horizontal";
    [SerializeField] public string vertical_move_input = "Vertical";
    [SerializeField] public KeyCode sprint_key = KeyCode.LeftShift;
    [SerializeField] public KeyCode jump_key = KeyCode.Space;
    [SerializeField] public KeyCode interact_key = KeyCode.E;
    [SerializeField] public KeyCode pause_key = KeyCode.Escape;

    [Header("Level Audio")]
    [SerializeField] public AudioSource player_music_source;
    [SerializeField] public AudioSource player_exposure_alert_source;
    [SerializeField] private AudioClip level_exposure_increase_alert;
    [SerializeField] private AudioClip level_exposure_decrease_alert;
    [SerializeField] private AudioClip[] level_music;
    [SerializeField] private int current_music;

    [Header("Global/Shared Variables")]
    public Player player;
    public float gravity = 9.18f;

    [Header("Level Specific")]
    [Header("Level 1")]
    [SerializeField] private float exp_increase_sprint = 0.007f;
    [SerializeField] private float decay = 0.00000000000000000000000000005f;
    [Header("Level 2")]
    [SerializeField] private int current_section = 1;
    [SerializeField] private Enemy_Knight[] activatable_knights;
    [SerializeField] private Enemy_Knight[] initial_knights;
    [SerializeField] private Enemy_Knight[] section_1_knights;
    [SerializeField] private Enemy_Knight[] section_2_knights;
    [SerializeField] private Enemy_Knight[] section_3_knights;
    [SerializeField] private Enemy_Knight[] secret_room_knights_1;
    [SerializeField] private Enemy_Knight[] secret_room_knights_2;

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
        HandlePauseInput();
        HandleLevel_1();
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //Later code here for other things
    }

    //Pause
    public void game_pause()
    {
        //Time scale
        Time.timeScale = 0f;
    }

    //Unpause
    public void game_unpause()
    {
        //Time scale
        Time.timeScale = 1f;
    }

    //Return player
    public Player return_player()
    {
        return player;
    }

    //PAUSE
    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(pause_key))
        {
            UnityEngine.Debug.Log("ESC detected");
            //Check if already paused
            if (paused)
            {
                //Unpause
                game_unpause();
                LockCursor();
                paused = false;
                //Enable input
                player.allow_input = true;
            }
           else
            {
                //Pause
                game_pause();
                UnlockCursor();
                paused = true;
                //Disable input
                player.allow_input = false;
            }
        }
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

    //LEVEL 1
    void HandleLevel_1()
    {
        if (level_int == 1)
        {
            //Check if player sprinting
            if (player.isSprinting())
            {
                //Increase exposure
                increase_exposure_amount(exp_increase_sprint);
            }
            else
            {
                //Drain exposure
                decrease_exposure_amount(decay);
            }
        }
    }

    //LEVEL 2
    public bool check_if_knight_activatable(Enemy_Knight knight)
    {
        return activatable_knights.Contains(knight);
    }

    public void activate_initial_knights()
    {
        Enemy_Knight[] merged_knights = activatable_knights.Concat(initial_knights).ToArray();
        activatable_knights = merged_knights;
    }

    public void activate_section_knights(int x)
    {

        Enemy_Knight[] merged_knights = null;

        switch (x)
        {
            case 1:
                merged_knights = activatable_knights.Concat(section_1_knights).ToArray();
                activatable_knights = merged_knights;
                break;
            case 2:
                merged_knights = activatable_knights.Concat(section_2_knights).ToArray();
                activatable_knights = merged_knights;
                break;
            case 3:
                merged_knights = activatable_knights.Concat(section_3_knights).ToArray();
                activatable_knights = merged_knights;
                break;
            case 4:
                //Secret room_1
                merged_knights = activatable_knights.Concat(secret_room_knights_1).ToArray();
                activatable_knights = merged_knights;
                break;
            case 5:
                //Secret room_2
                merged_knights = activatable_knights.Concat(secret_room_knights_2).ToArray();
                activatable_knights = merged_knights;
                break;
            default:
                //Youre not supposed to be here
                break;
        }
    }
}
