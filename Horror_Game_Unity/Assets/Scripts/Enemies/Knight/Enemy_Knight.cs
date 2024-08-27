using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

public class Enemy_Knight : Enemy
{
    //Knight unique booleans
    [Header("Unique")]
    //0 is inactive
    //1 is active
    [SerializeField] private int current_active_status = 0;
    //Set to level it actives on
    [SerializeField] private int activation_level;
    [SerializeField] public Light eye_light_1;
    [SerializeField] public Light eye_light_2;
    [SerializeField] public Light armor_light;
    [SerializeField] public Enemy_Knight_Weapons knight_weapons;
    [SerializeField] public float speed_0 = 2.1f;
    [SerializeField] public float speed_1 = 2.3f;
    [SerializeField] public float speed_2 = 2.6f;
    [SerializeField] public float speed_3 = 1.8f;
    [SerializeField] private Camera main_camera;
    [SerializeField] private Plane[] cameraFrustum;
    [SerializeField] private Collider collider_main;
    [SerializeField] public bool camera_seen;

    // Start
    public override void Start()
    {
        //No Nav-Mesh Agent
        agent.enabled = false;

        //Setup
        can_attack = false;
        Player player = level.player;
        last_known_player_location = player.gameObject.transform.position;
        agent.avoidancePriority = UnityEngine.Random.Range(30, 60);

        //Get main character from player
        main_camera = player.main_camera;
        //Set camera box
        collider_main = collider_main.GetComponent<Collider>();

        //AUDIO
        move_sound_interval = 0.70f;
        next_step_time = 0f;
        chatter_mininterval = 3f;
        chatter_maxinterval = 8f;
        chatter_playchance = 0.25f;
        chatter_nextchattertime = 0f;

        //Camera Effects
        targetRotation = new Vector3(-15f, 0f, 0f);
        attacked_move_duration = 1.0f;
        attacked_shake_magnitude = 0.1f;
        attacked_shake_frequency = 5f;
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
            //Unfreeze Animation                   
            animator.speed = 1.0f;

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
                        //UnityEngine.Debug.Log("Deactivated");
                    }
                    else
                    {
                        //UnityEngine.Debug.Log("Still Deactivating");
                    }
                }

                else
                {
                   // UnityEngine.Debug.Log("Deactivating");
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
                //UnityEngine.Debug.Log("Not Idling");
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
                        //UnityEngine.Debug.Log("Activated");
                        //Set hunting state
                        current_active_status = 3;
                        can_attack = true;
                    }
                    else
                    {
                        //UnityEngine.Debug.Log("Still Activating");
                    }
                }
                else
                {
                    animator.SetBool("Inactive", false);
                    animator.SetBool("Becoming_Active", true);
                    //UnityEngine.Debug.Log("Becoming Activated");
                }
            }
            if (anim_info.IsName("Active_idle"))
            {
                //Proceed to status 3
                current_active_status = 3;
            }
        }
        
        //If not attacking
        if (current_active_status == 2)
        {
            //Unfreeze Animation                   
            animator.speed = 1.0f;
            //Remove navigation
            agent.enabled = false;
            //Check if attacking already
            if (anim_info.IsName("Attack"))
            {
                //Special case
                if (anim_info.normalizedTime >= 0.60f && anim_info.normalizedTime <= 0.90f)
                {
                    //Apply camera shake
                    Player player = level.player;
                    player.CameraAttackedShake(targetRotation, attacked_move_duration, attacked_shake_magnitude, attacked_shake_frequency);
                }

                //Check if finished
                if (anim_info.normalizedTime >= 1.0f)
                {
                    //Walkround
                    Vector3 rotation = transform.rotation.eulerAngles;
                    transform.rotation = Quaternion.Euler(0, rotation.y, rotation.z);
                    //Play sound
                    knight_weapons.playHitSound();
                    UnityEngine.Debug.Log("Finished Attacking");
                    //If finished switch around
                    animator.SetBool("Attacking", false);
                    animator.SetBool("Walking", false);
                    //Set back to idle state
                    current_active_status = 1;
                    isattacking = false;
                    can_attack = true;
                }
            }
            else
            {
                animator.SetBool("Attacking", true);
                animator.SetBool("Walking", false);
                UnityEngine.Debug.Log("Start Attacking");
                //Turn towards player
                Player player = level.player;
                transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                //Walkround
                Vector3 rotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(0, rotation.y, rotation.z);
            }
        }

        //If not walking
        if (current_active_status == 3)
        {
            //Walk
            animator.SetBool("Walking", true);

            //Now check exposure level and if attacking
            if (level.exposure_level < 3 && isattacking == false)
            {
                //Freeze animation
                if (camera_seen == true)
                {
                    //Time top freeze animations
                    animator.speed = 0.0f;
                }
                else
                {
                    //Unfreeze animations
                    animator.speed = 1.0f;
                }
            }
            else
            {
                //No freeze
                //Unfreeze animations
                animator.speed = 1.0f;
            }
        }
    }

    //Handle nvigation
    public override void Handle_Navigation()
    {
        //Specific activation levels
        //First detect if exposure is less than 3 and not attacking
        if (level.exposure_level < 3 && current_active_status != 2)
        {
            //Then check if they are seen
            if (camera_seen == true)
            {
                //Disable movement
                agent.enabled = false;

                //Freeze Animation
            }
            //Camera doesn't see them
            else
            {
                //If hunting state and not attacking
                if (current_active_status == 3 && current_active_status != 2)
                {
                    UpdatePlayer();
                    can_attack = true;
                }
            }
        }
        //If attacking
        if (current_active_status == 2)
        {
            //Disable movement
            agent.enabled = false;
            can_attack = false;
        }

        //If it is equal to than 3
        //Greater is failsafe
        if (level.exposure_level >= 3 && current_active_status != 2)
        {
            //Check if attacking
            if (isattacking == false)
            {
                //Hunt player
                UpdatePlayer();
                can_attack = true;
            }

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
    }

    //Handle animation
    public override void Handle_Behaviour()
    {
        //Check if allowed to activate
        bool can_activate = level.check_if_knight_activatable(this);
        //if can cactivate
        if (can_activate)
        {
            if (level.exposure_level <= activation_level && current_active_status == 0)
            {
                current_active_status = 1;
            }

            if (level.exposure_level < activation_level && current_active_status != 0)
            {
                current_active_status = 0;
            }
        }

        //Workaround
        if (current_active_status == 0)
        {
            can_attack = false;
        }

    }

    public override void Handle_Audio()
    {
        //Handle Footsteps
        if (current_active_status == 3 && Time.time > next_step_time)
        {
            //Check if it is level 3
            if (level.exposure_level == 3)
            {
                //Play footsteps
                PlayFootstep();
                next_step_time = Time.time + move_sound_interval;
            }
            //If not level 3
            else
            {
                //Check if they are seen
                if (camera_seen == false)
                {
                    //Play footsteps
                    PlayFootstep();
                    next_step_time = Time.time + move_sound_interval;
                }
                else
                {
                    //Stop playing
                    move_source.Stop();
                }

            }

        }
        if (current_active_status != 3)
        {
            //Stop playing
            move_source.Stop();
        }

        //Handle Chatter
        //Dummy bool
        bool chatter = false;
        if (current_active_status >= 1)
        {
            //Check if it is level 3
            if (level.exposure_level == 3)
            {
                chatter = true;
            }
            else
            {
                //Check if not seen
                if (camera_seen == false)
                {
                    chatter = true;
                }
            }
        }

        //Chatter
        //Randomize it
        if (chatter == true)
        {
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

    //Attack
    public override void Handle_Attack()
    {
        if (isattacking == true)
        {
            //Set state to 2
            current_active_status = 2;
            UnityEngine.Debug.Log("State 2");
            can_attack = false;
        }
    }

    public override void Handle_Paused()
    {
        //Stop playing audio
        move_source.Stop();
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

        // Define a layer mask for the environment layer (layer 11)
        int environmentLayerMask = 1 << 11;

        // Handle if camera sees them
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(main_camera);
        var bounds = collider_main.bounds;

        if (GeometryUtility.TestPlanesAABB(cameraFrustum, bounds))
        {
            // Calculate the center point of the enemy's collider
            Vector3 enemyCenter = bounds.center;

            // Raycast from the camera to the center of the enemy's collider
            Vector3 directionToEnemy = enemyCenter - main_camera.transform.position;
            float distanceToEnemy = directionToEnemy.magnitude;
            Ray ray = new Ray(main_camera.transform.position, directionToEnemy.normalized);

            // Draw the ray in the scene view for debugging (optional)
            UnityEngine.Debug.DrawRay(ray.origin, ray.direction * distanceToEnemy, Color.red, 2f);
            //4UnityEngine.Debug.Log("Collider main: " + collider_main.name);

            // Perform the raycast with the environment layer mask
            if (Physics.Raycast(ray, out RaycastHit hit, distanceToEnemy, environmentLayerMask))
            {
                // Debug log for raycast hit details
                //UnityEngine.Debug.Log("Raycast hit: " + hit.collider.name + ", Distance: " + hit.distance);

                if (hit.collider == collider_main)
                {
                    //UnityEngine.Debug.Log("Enemy is in frustum and visible");
                    camera_seen = true;
                }
                else
                {
                    //UnityEngine.Debug.Log("Enemy is in frustum but obstructed by: " + hit.collider.name);
                    camera_seen = false;
                }
            }
            else
            {
                //UnityEngine.Debug.Log("Enemy is in frustum with a direct line of sight");
                camera_seen = true;
            }
        }
        else
        {
            //UnityEngine.Debug.Log("Enemy is not in frustum");
            camera_seen = false;
        }

    }

    //AI
    void UpdatePlayer()
    {
        agent.enabled = true;
        //Set to player location
        Player player = level.player;
        agent.SetDestination(player.gameObject.transform.position);
        //Turn towards player
        //Calculate direction
        //UnityEngine.Debug.Log("Destination set to: " + agent.destination);
        //UnityEngine.Debug.Log("Agent speed: " + agent.speed);
        //UnityEngine.Debug.Log("Agent is stopped: " + agent.isStopped);
        Vector3 direction = player.gameObject.transform.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.07f);
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


