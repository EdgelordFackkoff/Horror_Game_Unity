using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walk_speed = 3.0f;
    [SerializeField] private float sprint_multipler = 5.0f;
    [SerializeField] private float jump_force = 3.0f;
    //This will not change
    private float gravity;

    [Header("Input")]
    [SerializeField] private bool allow_input = true;
    [SerializeField] private float mouse_sensitivity;
    [SerializeField] private float up_down_range = 80.0f;
    private string horizontal_move_input;
    private string vertical_move_input;
    private KeyCode sprint_key;
    private KeyCode jump_key;
    private KeyCode interact_key;
    //No one in their right minds will change this
    [SerializeField] private string mouse_x_input = "Mouse X";
    [SerializeField] private string mouse_y_input = "Mouse Y";
    [SerializeField] private float vertical_rotation;

    [Header("Gameplay")]
    [SerializeField] private bool temp_invul = false;
    [SerializeField] private int temp_invul_time = 2;
    [SerializeField] private float attacked_lockduration = 0.8f;
    [SerializeField] private float hitpoints = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float stamina_drain_rate = 15.00f;
    [SerializeField] private float stamina_regain_rate = 5.00f;
    [SerializeField] private float stamina_run_threshold = 30.0f;
    [SerializeField] private bool can_sprint = true;
    [SerializeField] private bool is_interacting = false;
    [SerializeField] private Interactable current_interacting;
    [SerializeField] public bool player_alive = true;

    [Header("Audio")]
    [Header("Game Music")]
    [SerializeField] public AudioSource game_music_source;
    [Header("Alerts")]
    [SerializeField] public AudioSource exposure_alert_source;
    [Header("Pain/Grunt")]
    [SerializeField] public AudioSource on_hit_grunt_source;
    [SerializeField] private AudioClip[] on_hit_grunts_sounds;
    [SerializeField] public AudioSource pain_loop_source;
    [SerializeField] private AudioClip[] pain_loop_sounds;
    [Header("Footsteps")]
    [SerializeField] public AudioSource footstep_source;
    [SerializeField] private AudioClip[] footstep_sounds;
    [SerializeField] private float walk_interval = 0.5f;
    [SerializeField] private float sprint_interval = 0.3f;
    [SerializeField] private float velocity_threshold = 2.0f;

    [Header("Camera")]
    //Main Camera control
    [SerializeField] public GameObject camera_attachment;
    [SerializeField] public Transform camera_attachment_initial_position;
    [SerializeField] public Camera main_camera;
    [SerializeField] public bool attacked_camera_shaking;


    //Current movement
    //Starts with none
    private Vector3 current_movement = Vector3.zero;
    //Get state
    private bool is_moving;
    private bool is_sprinting;
    //For Audio
    private float next_step_time;
    private int last_played_footstep = -1;
    [Header("References")]
    //CharacterController variable
    public CharacterController character_controller;
    //Player UI variable
    public Canvas player_canvas;
    private Player_UI player_ui;
    public GameObject damage_hitbox;
    public GameObject interact_hitbox;
    //Level variable
    private Level level;

    //start
    private void Start()
    {
        //Reference Level
        level = GetComponentInParent<Level>();
        //Reference Player UI Script
        player_ui = player_canvas.GetComponent<Player_UI>();
        //Get gravity
        gravity = level.gravity;
        //Reference initial camera_attachment position grab
        camera_attachment_initial_position = camera_attachment.transform;

        //Run update input
        UpdateInputs();
    }

    //Lock cursor
    private void LockCursor()
    {
        level.LockCursor();
    }

    //update
    private void Update()
    {
        //Check input allowed
        if (allow_input == true && player_alive == true)
        {
            HandleMovement();
            HandleRotation();
            HandleFootsteps();
            HandleInteract();
        }
        HandleHealthStamina();
        //Give UI can sprint or not info
        player_ui.SetSprintAllowed(can_sprint);
    }

    //Update Inputs
    void UpdateInputs()
    {
        //Get inputs from Level
        mouse_sensitivity = level.mouse_sensitivity;
        horizontal_move_input = level.horizontal_move_input;
        vertical_move_input = level.vertical_move_input;
        sprint_key = level.sprint_key;
        jump_key = level.jump_key;
        interact_key = level.interact_key;
    }

    //Handle Movement
    void HandleMovement()
    {
        //Dummy
        float speed_multiplier = 1.0f;

        //Constantly check the changes in movement inputs
        float vertical_input = Input.GetAxis(vertical_move_input);
        float horizontal_input = Input.GetAxis(horizontal_move_input);

        //See if you can sprint
        if (can_sprint == true)
        {
            //Detect if sprint key is pressed
            if (Input.GetKey(sprint_key) == true && is_moving == true && character_controller.isGrounded == true)
            {
                speed_multiplier = sprint_multipler;
                is_sprinting = true;

            }
            //Not sprinting
            else
            {
                is_sprinting = false;
            }
        }
        //Not sprinting
        else
        {
            is_sprinting = false;
            speed_multiplier = 1.0f;
        }

        float vertical_speed = vertical_input * walk_speed * speed_multiplier;
        float horizontal_speed = horizontal_input * walk_speed * speed_multiplier;

        //Create a new Vector 3
        //Horizontal Movement
        Vector3 horizontal_movement = new Vector3(horizontal_speed, 0, vertical_speed);
        horizontal_movement = transform.rotation * horizontal_movement;
        //Handle Gravity
        HandleJumping();

        //Combine horizontal and vertical
        current_movement.x = horizontal_movement.x;
        current_movement.z = horizontal_movement.z;

        //MOVE THE CHARACTER
        character_controller.Move(current_movement * Time.deltaTime);

        //Is moving check
        is_moving = vertical_input != 0 || horizontal_input != 0;
    }
    
    void HandleJumping()
    {
        //Check if grounded
        if (character_controller.isGrounded)
        {
            current_movement.y = -0.5f;

            if (Input.GetKeyDown(jump_key))
            {
                current_movement.y = jump_force;
            }
        }
        //In the air now
        else
        {
            current_movement.y -= gravity * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        //Get mouse x axis input
        float mouse_x_rotation = Input.GetAxis(mouse_x_input) * mouse_sensitivity;
        transform.Rotate(0, mouse_x_rotation, 0);

        //Get vertical rotation
        vertical_rotation -= Input.GetAxis(mouse_y_input) * mouse_sensitivity;
        //Clamp the rotation
        vertical_rotation = Mathf.Clamp(vertical_rotation, -up_down_range, up_down_range);
        main_camera.transform.localRotation = Quaternion.Euler(vertical_rotation, 0, 0);
    }

    void HandleFootsteps()
    {
        float current_step_interval = walk_interval;

        //Check if sprinting
        if (is_sprinting == true)
        {
            current_step_interval = sprint_interval;
        }

        //If on ground
        if (character_controller.isGrounded && is_moving && Time.time > next_step_time && character_controller.velocity.magnitude > velocity_threshold)
        {
            //Play footsteps
            PlayFootstep();
            next_step_time = Time.time + current_step_interval;
        }
    }

    void HandleHealthStamina()
    {

        //Handle Stamina
        //Detect if you're sprinting
        if (is_sprinting == true)
        {
            //Check if your stamina allows
            if (stamina >= 1.0f)
            {
                //Drain stamina
                stamina -= stamina_drain_rate * Time.deltaTime;
                player_ui.ResetStaminaLerp();
            }

            //No sprinting for you
            if (stamina < 1.0f)
            {
                can_sprint = false;
                UnityEngine.Debug.Log("Can no longer sprint");
                player_ui.ResetStaminaLerp();
            }

        }
        //If you aren't sprinting
        else
        {

            //Regain stamina
            stamina += stamina_regain_rate * Time.deltaTime;

            //If stamina above a threshold
            if (stamina >= stamina_run_threshold && can_sprint == false)
            {
                //Allow sprinting again
                UnityEngine.Debug.Log("Can sprint again");
                can_sprint = true;
            }

            //Lerp constantly
            player_ui.ResetStaminaLerp();
        }

        //Clamp values
        hitpoints = Mathf.Clamp(hitpoints, 0, 100.0f);
        stamina = Mathf.Clamp(stamina, 0, 100.0f);
        //Update the hitpoint and stamina for UI
        player_ui.UpdateUIHealthStamina(hitpoints, stamina);
    }

    void HandleInteract()
    {
        if (current_interacting != null)
        {
            //Detect if instant or normal
            if (current_interacting.instant_interact() == true)
            {
                if (Input.GetKeyDown(interact_key))
                {
                    current_interacting.effect();
                    UnityEngine.Debug.Log("Instant interact");
                }
            }
            else
            {
                if (Input.GetKey(interact_key))
                {
                    current_interacting.effect();
                    UnityEngine.Debug.Log("Non-Instant interact");
                }
            }
        }
    }


    //Handle Interact
    public void HandleInteractableEnter(Collider other)
    {
        current_interacting = other.GetComponent<Interactable>();
    }

    public void HandleInteractableStay(Collider other)
    {
        current_interacting = other.GetComponent<Interactable>();
    }

    public void HandleInteractableExit(Collider other)
    {
        current_interacting = null;
    }

    public void HandleInteractableNone()
    {
        current_interacting = null;
    }

    public Interactable getCurrentInteractable()
    {
        return current_interacting;
    }

    //DAMAGE
    public void TakeDamage(float damage)
    {
        //Check if damage is fatal
        if (hitpoints - damage <= 0)
        {
            //Fatal damage
            //Die
            hitpoints = 0;
            allow_input = false;
            temp_invul = false;
            player_ui.ResetHealthLerp();
            player_alive = false;
            current_interacting = null;
            //Tell UI to begin death animation
            player_ui.player_death();

        }
        else
        {
            //Get hit
            hitpoints -= damage;
            hitpoints = Mathf.Clamp(hitpoints, 0, 100);
            player_ui.ResetHealthLerp();
        }
    }

    //HEAL
    public void HealDamage(float heal)
    {
        //Get hit
        hitpoints += heal;
        hitpoints = Mathf.Clamp(hitpoints, 0, 100);
        player_ui.ResetHealthLerp();
    }

    //AUDIO
    void PlayFootstep()
    {
        //Randomize Footstep
        int random_index;
        //Just to idiotproof errors
        if (footstep_sounds.Length == 1)
        {
            random_index = 0;
        }
        else
        {
            random_index = UnityEngine.Random.Range(0, footstep_sounds.Length - 1);
            if (random_index >= last_played_footstep)
            {
                random_index++;
            }
        }

        //Randomize play
        last_played_footstep = random_index;
        footstep_source.clip = footstep_sounds[random_index];
        footstep_source.Play();
    }

    //Getting attacked
    public void Attacked(Enemy enemy)
    {
        //Check if invul false
        if (temp_invul == false && player_alive == true)
        {
            //Activate temp invul
            temp_invul = true;

            //Lock input
            allow_input = false;

            //Cancel sprint
            is_sprinting = false;


            // Capture the player's original rotation
            Quaternion original_rotation = character_controller.transform.rotation;

            // Calculate the direction vector from the camera to the target
            Vector3 direction = enemy.eye_level.position - character_controller.transform.position;

            // Calculate the target rotation to face the enemy's eye level
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate the camera to face the target
            character_controller.transform.rotation = Quaternion.Slerp(character_controller.transform.rotation, targetRotation, 0.5f * Time.deltaTime);

            //Make attacking true for enemy
            enemy.isAttacking_true();
            StartCoroutine(Lockattack(enemy, original_rotation, targetRotation));
        }
    }

    public void CameraAttackedShake(Vector3 targetRotation, float attackedMoveDuration, float attackedShakeMagnitude, float attackedShakeFrequency)
    {
        // Set camera to shaking
        attacked_camera_shaking = true;
        StartCoroutine(CameraShakeMove(targetRotation, attackedMoveDuration, attackedShakeMagnitude, attackedShakeFrequency));
    }

    IEnumerator CameraShakeMove(Vector3 targetRotation, float attackedMoveDuration, float attackedShakeMagnitude, float attackedShakeFrequency)
    {
        Quaternion initialRotation = camera_attachment_initial_position.localRotation;
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);

        Vector3 initialPosition = camera_attachment.transform.localPosition;

        float elapsedTime = 0.0f;

        while (elapsedTime < attackedMoveDuration)
        {
            // Interpolate towards the target rotation
            camera_attachment.transform.localRotation = Quaternion.Slerp(initialRotation, targetQuaternion, elapsedTime / attackedMoveDuration);

            // Apply shake effect to position
            float shakeOffsetX = Mathf.PerlinNoise(Time.time * attackedShakeFrequency, 0) * 2f * attackedShakeMagnitude - attackedShakeMagnitude;
            float shakeOffsetY = Mathf.PerlinNoise(0, Time.time * attackedShakeFrequency) * 2f * attackedShakeMagnitude - attackedShakeMagnitude;
            float shakeOffsetZ = Mathf.PerlinNoise(Time.time * attackedShakeFrequency, Time.time * attackedShakeFrequency) * 2f * attackedShakeMagnitude - attackedShakeMagnitude;

            camera_attachment.transform.localPosition = initialPosition + new Vector3(shakeOffsetX, shakeOffsetY, shakeOffsetZ);

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Optionally, dampen the shake magnitude over time
            attackedShakeMagnitude = Mathf.Lerp(attackedShakeMagnitude, 0f, elapsedTime / attackedMoveDuration);

            yield return null;
        }

        // Ensure the camera_attachment is exactly at the target rotation and initial position
        camera_attachment.transform.localRotation = targetQuaternion;
        camera_attachment.transform.localPosition = initialPosition;

        // Pause briefly before returning to the original rotation
        yield return new WaitForSeconds(0.3f);

        // Return to original rotation and position
        elapsedTime = 0f;
        while (elapsedTime < attackedMoveDuration + 0.7)
        {
            camera_attachment.transform.localRotation = Quaternion.Slerp(targetQuaternion, initialRotation, elapsedTime / attackedMoveDuration);
            camera_attachment.transform.localPosition = initialPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera_attachment is exactly at the original rotation and position
        camera_attachment.transform.localRotation = initialRotation;
        camera_attachment.transform.localPosition = initialPosition;

        // Reset shake magnitude and state
        attacked_camera_shaking = false;
    }

    IEnumerator Lockattack(Enemy enemy, Quaternion originalRotation, Quaternion targetRotation)
    {
        float elapsedTime = 0f;

        while (elapsedTime < attacked_lockduration)
        {
            //Rotate player towards enemy
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedTime / attacked_lockduration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        main_camera.transform.LookAt(enemy.eye_level.transform);

        //Get damaged
        float enemy_damage = enemy.get_damage();
        TakeDamage(enemy_damage);

        //Show shield
        player_ui.show_hide_shield(1);

        //Return to original rotation
        elapsedTime = 0f;
        while (elapsedTime < attacked_lockduration)
        {
            // Rotate player towards enemy
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, elapsedTime / attacked_lockduration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        main_camera.transform.rotation = originalRotation;

        //Reenable character input and movement
        allow_input = true;
        //Start countdown for invul
        StartCoroutine(Invul_cooldown());
    }

    IEnumerator Invul_cooldown()
    {
        for (int i = temp_invul_time; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
        }

        //UnityEngine.Debug.Log("Invul over");
        //Hide shield
        player_ui.show_hide_shield(0);
        temp_invul = false;
    }
}
