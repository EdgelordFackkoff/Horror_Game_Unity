using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dancer : Enemy
{
    [Header("Information")]
    [SerializeField] private bool is_moving;
    [SerializeField] private float timer;

    // NOT IMPLEMENTED YET
    [SerializeField] private float speed_0 = 5;
    [SerializeField] private float speed_1 = 7;
    [SerializeField] private float speed_2 = 9;
    [SerializeField] private float speed_3 = 11;

    [SerializeField] private float speed_dancing = 3;

    Player player;
    public DancerManager dancer_manager;

    public enum State
    {
        Dancing,
        Not_Dancing,
        Perma_Active // maybe will not implement
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

    }
    public override void Handle_Navigation()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case State.Dancing:
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
                if (is_moving)
                {
                    StopMoving();
                    is_moving = false;
                }
                if (player.GetComponent<CharacterController>().velocity.magnitude > 0.1f)
                {
                    Chasing();
                }
                else if (player.GetComponent<CharacterController>().velocity.magnitude < 0.1f)
                {
                    StopMoving();
                }
                break;
            case State.Perma_Active:

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
            if (dancer_manager.is_music_playing)
            {
                agent.speed = speed_dancing;
                state = State.Dancing;
            }
            else if (!dancer_manager.is_music_playing)
            {
                agent.speed = speed_0;
                state = State.Not_Dancing;
            }
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
}
