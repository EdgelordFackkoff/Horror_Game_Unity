using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Enemy_Knight : Enemy
{
    //Knight unique booleans
    [Header("Unique")]
    //0 is inactive
    //1 is active
    [SerializeField] private int current_active_status = 0;
    [SerializeField] public Light eye_light_1;
    [SerializeField] public Light eye_light_2;
    [SerializeField] public Light armor_light;
    [SerializeField] public Enemy_Knight_Weapons knight_weapons;
    [SerializeField] public float speed_0 = 1.2f;
    [SerializeField] public float speed_1 = 1.4f;
    [SerializeField] public float speed_2 = 1.6f;
    [SerializeField] public float speed_3 = 1.8f;

    // Start
    public override void Start()
    {
        //No Nav-Mesh Agent
        agent.enabled = false;

        //Setup
        Player player = level.player;
        last_known_player_location = player.gameObject.transform.position;
        agent.avoidancePriority = UnityEngine.Random.Range(30, 60);

        //AUDIO
        move_sound_interval = 0.70f;
        next_step_time = 0f;
        chatter_mininterval = 3f;
        chatter_maxinterval = 8f;
        chatter_playchance = 0.25f;
        chatter_nextchattertime = 0f;

    }

    //Activate or Deactivae Knight
    public void activate_knight(int x)
    {
        if (x == 1 || x == 2)
        {
            current_active_status = x;
        }
    }

    //Handle animation
    public override void Handle_Animation()
    {
        //See current animation
        AnimatorStateInfo anim_info = animator.GetCurrentAnimatorStateInfo(0);


        //Go through evet statuses
        if (current_active_status == 0)
        {
            //Deactivate NavMesh
            agent.enabled = false;

            UnityEngine.Debug.Log("Not Idling(Inactive)");
            //If already playing
            if (!anim_info.IsName("Inactive_idle"))
            {
                //See if deactivation animation is playing
                if (anim_info.IsName("Become_InActive"))
                {
                    //Check if finished
                    if (anim_info.normalizedTime >= 1.0f)
                    {
                        //If  finished switch around
                        animator.SetBool("Inactive", true);
                        animator.SetBool("Becoming_InActive", false);
                        UnityEngine.Debug.Log("Deactivated");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Still Deactivating");
                    }
                }

                else
                {
                    UnityEngine.Debug.Log("Deactivating");
                    animator.SetBool("Walking", false);
                    animator.SetBool("Active", false);
                    animator.SetBool("Becoming_InActive", true);
                }
            }

            else
            {
                //Nothing to change
            }

        }

        if (current_active_status == 1)
        {
            if (!anim_info.IsName("Active_idle") && !anim_info.IsName("Walk_Loop"))
            {
                UnityEngine.Debug.Log("Not Idling");
                //See if activating animation is playing
                if (anim_info.IsName("Become_Active"))
                {
                    //Check if finished
                    if (anim_info.normalizedTime >= 1.0f)
                    {
                        //If finished switch around
                        animator.SetBool("Inactive", false);
                        animator.SetBool("Active", true);
                        animator.SetBool("Becoming_Active", false);
                        UnityEngine.Debug.Log("Activated");
                        //Set hunting state
                        current_active_status = 3;
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Still Activating");
                    }
                }
                else
                {
                    animator.SetBool("Inactive", false);
                    animator.SetBool("Becoming_Active", true);
                    UnityEngine.Debug.Log("Becoming Activated");
                }
            }
        }

        //If not walking
        if (current_active_status == 3)
        {
            //Walk boy
            animator.SetBool("Walking", true);
        }

    }

    //Handle nvigation
    public override void Handle_Navigation()
    {

    }

    //Handle animation
    public override void Handle_Behaviour()
    {
        //Some changes to test
        if (level.exposure_level >= 1 && current_active_status == 0)
        {
            current_active_status = 1;
        }

        if (level.exposure_level < 1 && current_active_status != 0)
        {
            current_active_status = 0;
        }

        //Change speed based on level
        switch (level.exposure_level)
        {
            case 0:
                agent.speed = speed_0;
                break;
            case 1:
                agent.speed = speed_1;
                break;
            case 2:
                agent.speed = speed_2;
                break;
            case 3:
                agent.speed = speed_3;
                break;
        }

        //Specific activation levels
        //Hunting state
        if (current_active_status == 3)
        {
            agent.enabled = true;
            //Set to player location
            Player player = level.player;
            agent.SetDestination(player.gameObject.transform.position);
            //Turn towards player
            //Calculate direction
            Vector3 direction = player.gameObject.transform.position - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.07f);

        }

    }

    public override void Handle_Audio()
    {
        //Handle Footsteps
        if (current_active_status == 3 && Time.time > next_step_time)
        {
            //Play footsteps
            PlayFootstep();
            next_step_time = Time.time + move_sound_interval;
        }

        //Handle Chatter
        if (current_active_status >= 1)
        {
            //Chatter
            if (Time.time >= chatter_nextchattertime)
            {
                // Randomly decide whether to play a chatter sound
                if (UnityEngine.Random.value <= chatter_playchance)
                {
                    PlayRandomChatter();
                }

                // Set the time for the next possible chatter sound
                chatter_nextchattertime = Time.time + UnityEngine.Random.Range(chatter_mininterval, chatter_maxinterval);
            }
        }
    }

    //Handle 
    public override void Handle_Misc()
    {
        //Handle light's color
        switch (level.exposure_level)
        {
            case 1:
                eye_light_1.color = Color.Lerp(eye_light_1.color, Color.green, 0.005f);
                eye_light_2.color = Color.Lerp(eye_light_2.color, Color.green, 0.005f);
                armor_light.color = Color.Lerp(armor_light.color, Color.green, 0.005f);
                break;
            case 2:
                eye_light_1.color = Color.Lerp(eye_light_1.color, Color.yellow, 0.005f);
                eye_light_2.color = Color.Lerp(eye_light_2.color, Color.yellow, 0.005f);
                armor_light.color = Color.Lerp(armor_light.color, Color.yellow, 0.005f);
                break;
            case 3:
                eye_light_1.color = Color.Lerp(eye_light_1.color, Color.red, 0.005f);
                eye_light_2.color = Color.Lerp(eye_light_2.color, Color.red, 0.005f);
                armor_light.color = Color.Lerp(armor_light.color, Color.red, 0.005f);
                break;
            default:
                break;
        }
    }

    //AUDIO
    void PlayFootstep()
    {
        //Randomize Footstep
        int random_index;
        //Just to idiotproof errors
        if (move_sounds.Length == 1)
        {
            random_index = 0;
        }
        else
        {
            random_index = UnityEngine.Random.Range(0, move_sounds.Length - 1);
            if (random_index >= last_played_move_sound)
            {
                random_index++;
            }
        }

        //Randomize play
        last_played_move_sound = random_index;
        move_source.clip = move_sounds[random_index];
        move_source.Play();
    }

    void PlayRandomChatter()
    {
        //Randomize Footstep
        int random_index;
        //Just to idiotproof errors
        if (chatter_sounds.Length == 1)
        {
            random_index = 0;
        }
        else
        {
            random_index = UnityEngine.Random.Range(0, chatter_sounds.Length - 1);
            if (random_index >= chatter_last_played_chatter_sound)
            {
                random_index++;
            }
        }

        //Randomize play
        chatter_last_played_chatter_sound = random_index;
        chatter_source.clip = chatter_sounds[random_index];
        chatter_source.Play();
    }
}


