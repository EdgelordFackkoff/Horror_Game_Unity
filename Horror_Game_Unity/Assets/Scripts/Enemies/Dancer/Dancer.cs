using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dancer : Enemy
{
    [Header("Information")]
    [SerializeField] private bool is_moving;
    [SerializeField] private float timer;

    [SerializeField] private float speed_0 = 5;
    [SerializeField] private float speed_1 = 7;
    [SerializeField] private float speed_2 = 9;
    [SerializeField] private float speed_3 = 11;

    public Transform reset_position;

    [SerializeField] private float speed_dancing = 2;

    Player player;
    public DancerManager dancer_manager;

    public enum State
    {
        Dancing,
        Not_Dancing,
        Attack
    };
    public State state;



    public override void Start()
    {
        //Setup
        player = level.player;
        last_known_player_location = player.gameObject.transform.position;

        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = UnityEngine.Random.Range(30, 60);
        agent.acceleration = 100;
        agent.autoBraking = false;
        agent.speed = speed_dancing;
        state = State.Dancing;

        is_moving = false;

        StartCoroutine(MusicChecker());

        //AUDIO
        move_sound_interval = 0.70f;
        next_step_time = 0f;
        chatter_mininterval = 3f;
        chatter_maxinterval = 8f;
        chatter_playchance = 0.25f;
        chatter_nextchattertime = 0f;

    }

    public override void Handle_Animation()
    {
        //AnimatorStateInfo anim_info = animator.GetCurrentAnimatorStateInfo(0);
    }
    public override void Handle_Navigation()
    {
        timer += Time.deltaTime;
        AnimatorStateInfo anim_info = animator.GetCurrentAnimatorStateInfo(0);

        switch (state)
        {
            case State.Dancing:
                animator.speed = 1f;
                animator.SetBool("Walking", true);

                if (isattacking)
                {
                    state = State.Attack;
                }

                if (!is_moving)
                {
                    RandomDestination();
                    is_moving = true;
                }
                // Checks for whether this has arrived to intended location or not
                else if (is_moving)
                {
                    if (!agent.pathPending)
                    {
                        if (agent.remainingDistance <= agent.stoppingDistance)
                        {
                            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0)
                            {
                                is_moving = false;
                            }
                        }
                    }
                }

                break;
            case State.Not_Dancing:
                animator.SetBool("Walking", true);

                if (isattacking)
                {
                    state = State.Attack;
                }

                if (is_moving)
                {
                    StopMoving();
                    is_moving = false;
                }
                if (player.GetComponent<CharacterController>().velocity.magnitude > 0.1f)
                {
                    animator.speed = 1f;
                    Chasing();
                }
                else if (player.GetComponent<CharacterController>().velocity.magnitude < 0.1f)
                {
                    animator.speed = 0f;
                    StopMoving();
                }
                break;
            case State.Attack:
                animator.SetBool("Attacking", true);

                //Unfreeze Animation                   
                animator.speed = 1.0f;
                //Remove navigation
                //agent.enabled = false;
                //Check if attacking already
                //if (anim_info.IsName("Attacking"))
                //{
                //    //Special case
                //    if (anim_info.normalizedTime >= 0.60f && anim_info.normalizedTime <= 0.90f)
                //    {
                //        //Apply camera shake
                //        Player player = level.player;
                //        player.CameraAttackedShake(targetRotation, attacked_move_duration, attacked_shake_magnitude, attacked_shake_frequency);
                //    }

                //    //Check if finished
                //    if (anim_info.normalizedTime >= 1.0f)
                //    {
                //        //Walkround
                //        Vector3 rotation = transform.rotation.eulerAngles;
                //        transform.rotation = Quaternion.Euler(0, rotation.y, rotation.z);
                //        //Play sound
                //        UnityEngine.Debug.Log("Finished Attacking");
                //        //If finished switch around
                //        animator.SetBool("Idling", false);
                //        animator.SetBool("Walking", false);
                //        animator.SetBool("Attacking", false);
                //        //Set back to idle state
                //        MusicChecker();
                //        isattacking = false;
                //        can_attack = true;
                //        transform.position = reset_position.position;
                //    }
                //}
                //else
                //{
                //    animator.SetBool("Idling", false);
                //    animator.SetBool("Walking", false);
                //    animator.SetBool("Attacking", true);
                //    UnityEngine.Debug.Log("Start Attacking");
                //    //Turn towards player
                //    Player player = level.player;
                //    transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                //    //Walkround
                //    Vector3 rotation = transform.rotation.eulerAngles;
                //    transform.rotation = Quaternion.Euler(0, rotation.y, rotation.z);
                //    MusicChecker();

                //}
                //MusicChecker();

                if (anim_info.IsName("Attack"))
                {
                    if (anim_info.normalizedTime >= 1.0f)
                    {
                        stateChecker();
                        Debug.Log("helpppp");
                        isattacking = false;
                        animator.SetBool("Attacking", false);
                        transform.position = reset_position.position;
                    }
                }


                break;
        }
    }
    public override void Handle_Behaviour()
    {

    }
    public override void Handle_Audio()
    {

    }
    public override void Handle_Misc()
    {

    }

    public override void Handle_Attack()
    {

    }

    public override void Handle_Paused()
    {

    }

    private IEnumerator MusicChecker()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            timer = 0;
            stateChecker();
        }
    }

    public Vector3 RandomNavMeshPosition(float radius, Vector3 pos)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius;
        randomDir += pos;
        NavMeshHit hit;
        Vector3 final_pos = Vector3.zero;
        if (NavMesh.SamplePosition(randomDir, out hit, radius, 1))
            final_pos = hit.position;
        return final_pos;
    }

    private void RandomDestination()
    {
        agent.destination = RandomNavMeshPosition(50, transform.position);
    }

    private void StopMoving()
    {
        agent.destination = transform.position;
    }

    private void Chasing()
    {
        last_known_player_location = player.gameObject.transform.position;
        agent.destination = last_known_player_location;
    }

    private void stateChecker()
    {
        if (dancer_manager.is_music_playing)
        {
            agent.speed = speed_dancing;
            state = State.Dancing;
        }
        else if (!dancer_manager.is_music_playing)
        {
            if (level.exposure_amount == 0)
                agent.speed = speed_0;
            if (level.exposure_amount == 1)
                agent.speed = speed_1;
            if (level.exposure_amount == 2)
                agent.speed = speed_2;
            if (level.exposure_amount == 3)
                agent.speed = speed_3;
            state = State.Not_Dancing;
        }
    }
}
