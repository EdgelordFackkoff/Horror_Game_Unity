using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Librarian : Enemy
{
    [Header("Information")]
    public float roam_interval;
    [SerializeField] private bool is_moving;
    [SerializeField] private float timer;

    // NOT IMPLEMENTED YET
    [SerializeField] private float speed_0 = 1;
    [SerializeField] private float speed_1 = 1.5f;
    [SerializeField] private float speed_2 = 2;
    [SerializeField] private float speed_3 = 2.5f;

    Player player;

    [Header("Sight")]
    public float aggro_radius;
    [Range(0f, 360f)]
    public float aggro_angle;
    public LayerMask target_mask;
    public LayerMask obstacle_mask;
    public List<Transform> target_list;

    public enum State
    {
        Chase,
        Roam,
        Roam_Near_Player
    };
    public State state;


    public override void Start()
    {
        //Setup
        player = level.player;
        last_known_player_location = player.gameObject.transform.position;

        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = UnityEngine.Random.Range(30, 60);

        state = State.Roam;
        is_moving = false;

        roam_interval = 10;

        aggro_radius = 10f;
        aggro_angle = 90f;

        // Will always check for targets in aggro_angle
        // State switch to chase is handled in here
        StartCoroutine(TargetChecker());

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
        if (state != State.Roam_Near_Player)
            timer += Time.deltaTime;

        switch (state)
        {
            case State.Chase:
                // Chase player
                StartCoroutine(Chasing());

                break;
            case State.Roam:
                // After some time, find and go to a position close to the player
                // Can be tweaked according to exposure level
                if (timer > roam_interval)
                {
                    timer = 0;
                    is_moving = false;
                    state = State.Roam_Near_Player;
                }

                // Move to a random location near itself
                if (!is_moving)
                {
                    StartCoroutine(RandomDestination());
                    is_moving = true;
                }

                break;
            case State.Roam_Near_Player:
                // Move to random position near player
                if (!is_moving)
                {
                    RandomDestinationNearPlayer();
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
                                Debug.Log("am here");
                                state = State.Roam;
                                is_moving = false;
                            }
                        }
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

    private IEnumerator RandomDestination()
    {
        agent.destination = RandomNavMeshPosition(15, transform.position);
        yield return new WaitForSeconds(5);
        is_moving = false;
    }

    private void RandomDestinationNearPlayer()
    {
        last_known_player_location = player.gameObject.transform.position;
        agent.destination = RandomNavMeshPosition(15, last_known_player_location);

        Debug.Log("not there");
    }

    public void FindVisibleTargets()
    {
        target_list.Clear();
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, aggro_radius, target_mask);

        // Find all targets in circle around itself
        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            Transform target = targetsInRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // Find all targets inside angle
            if (Vector3.Angle(transform.forward, dirToTarget) < aggro_angle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // Check if [LAYER] is blocking line of sight(raycast)
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacle_mask))
                {
                    // Add to list for checking
                    target_list.Add(target);
                    state = State.Chase;
                }
            }
        }
    }

    private IEnumerator TargetChecker()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            FindVisibleTargets();
        }
    }

    private IEnumerator Chasing()
    {
        // Checks if player is in list
        if (target_list.Count != 0)
        {
            last_known_player_location = player.gameObject.transform.position;
            agent.destination = last_known_player_location;
        }
        // Handling roam state switch
        else
        {
            state = State.Roam;
        }
        yield return new WaitForSeconds(0.2f);
    }

    public Vector3 DirFromAngle(float angle, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}