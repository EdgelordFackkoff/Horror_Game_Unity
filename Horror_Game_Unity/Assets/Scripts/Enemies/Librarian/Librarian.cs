using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class Librarian : Enemy
{
    [Header("Unique")]
    [SerializeField] public float roam_interval;
    [SerializeField] protected AudioSource smack_source;
    [SerializeField] private bool is_moving;
    [SerializeField] private int current_state;
    [SerializeField] private float timer;
    [SerializeField] private float speed_0 = 1.7f;
    [SerializeField] private float speed_1 = 2.0f;
    [SerializeField] private float speed_2 = 2.2f;
    [SerializeField] private float speed_3 = 2.4f;

    public override void Start()
    {
        //No Nav-Mesh Agent
        agent.enabled = false;

        //Setup
        can_attack = true;
        Player player = level.player;
        last_known_player_location = player.gameObject.transform.position;
        agent.avoidancePriority = UnityEngine.Random.Range(30, 60);
        current_state = 0;

        //AUDIO
        move_sound_interval = 0.60f;
        next_step_time = 0f;
        chatter_mininterval = 3f;
        chatter_maxinterval = 8f;
        chatter_playchance = 0.25f;
        chatter_nextchattertime = 0f;

        //Camera Effects
        targetRotation = new Vector3(-5f, 0f, 0f);
        attacked_move_duration = 1.0f;
        attacked_shake_magnitude = 0.1f;
        attacked_shake_frequency = 5f;

    }

    public override void Handle_Animation()
    {
        //See current animation
        AnimatorStateInfo anim_info = animator.GetCurrentAnimatorStateInfo(0);

        //Idle
        if (current_state == 0)
        {
            //Later check to see if it is active
            //Start hunting
            current_state = 1;
        }

        //Hunting
        if (current_state == 1)
        {
            //Start Moving
            current_state = 2;
        }

        //Walking
        if (current_state == 2)
        {
            //Walk
            if (anim_info.IsName("Walk"))
            {
                //Nothing, you're walking
            }
            //Start walking
            else
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Walk", true);
            }
        }

        //If attacking
        if (current_state == 3)
        {
            if (anim_info.IsName("Attack"))
            {
                //Special case
                if (anim_info.normalizedTime >= 0.45f && anim_info.normalizedTime <= 0.50f)
                {
                    //Apply camera shake
                    Player player = level.player;
                    player.CameraAttackedShake(targetRotation, attacked_move_duration, attacked_shake_magnitude, attacked_shake_frequency);

                    //Smack Sound
                    PlaySmackSound();
                }

                //Check if finished
                if (anim_info.normalizedTime >= 1.0f)
                {
                    //Walkround
                    Vector3 rotation = transform.rotation.eulerAngles;
                    transform.rotation = Quaternion.Euler(0, rotation.y, rotation.z);
                    //Play sound
                    UnityEngine.Debug.Log("Finished Attacking");
                    //If finished switch around
                    animator.SetBool("Idle", true);
                    animator.SetBool("Attacking", false);
                    animator.SetBool("Walk", false);
                    //Set back to idle state
                    current_state = 0;
                    isattacking = false;
                    can_attack = true;
                }
            }
            else
            {
                animator.SetBool("Attacking", true);
                animator.SetBool("Walk", false);
                UnityEngine.Debug.Log("Start Attacking");
                //Turn towards player
                Player player = level.player;
                transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                //Walkround
                Vector3 rotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(0, rotation.y, rotation.z);
            }
        }
    }

    public override void Handle_Navigation()
    {

        //If hunting
        if (current_state == 1 || current_state == 2)
        {
            //Update Player location
            UpdatePlayer();
        }

        //If attacking
        if (current_state == 3)
        {
            agent.enabled = false;
        }

    }
    public override void Handle_Behaviour()
    {

    }

    public override void Handle_Audio()
    {
        //Handle Footsteps
        if (current_state == 2 && Time.time > next_step_time)
        {
            //Play footsteps
            PlayFootstep();
            next_step_time = Time.time + move_sound_interval;
        }
        else if (current_state != 2)
        {
            //Stop playing
            move_source.Stop();
        }

        //Handle Chatter
        //Chatter
        if (Time.time >= chatter_nextchattertime)
        {
            // Randomly decide whether to play a chatter sound
       
            if (UnityEngine.Random.value <= chatter_playchance)
            {
                //Random Chatter Played
                UnityEngine.Debug.Log("Chatter Played");
                PlayRandomChatter();
            }
            // Set the time for the next possible chatter sound
            chatter_nextchattertime = Time.time + UnityEngine.Random.Range(chatter_mininterval, chatter_maxinterval);
        }
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

    void PlaySmackSound()
    {
        smack_source.Play();
    }

    public override void Handle_Attack()
    {
        if (isattacking == true)
        {
            //Set state to 3
            current_state = 3;
            can_attack = false;
        }
    }

    public override void Handle_Misc()
    {

    }

    public override void Handle_Paused()
    {

    }

    void UpdatePlayer()
    {
        agent.enabled = true;
        //Set to player location
        Player player = level.player;
        //UnityEngine.Debug.Log(player);
        agent.SetDestination(player.gameObject.transform.position);
        //Turn towards player
        //Calculate direction
        Vector3 direction = player.gameObject.transform.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1.00f);
    }
}