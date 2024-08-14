using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Librarian : Enemy
{
    [Header("Information")]
    public Transform player_transform;

    public enum State
    {
        Chase,
        Roam
    };
    public State state;

    private bool is_moving;


    // Start is called before the first frame update
    void Start()
    {
        player_transform = GameObject.FindWithTag("Player").transform;
        state = State.Roam;

        agent = GetComponent<NavMeshAgent>();
        //level_gameobject = GameObject.FindWithTag("Level");
        //level = level_gameobject.GetComponent<Level>();

        is_moving = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.Chase:
                agent.destination = player_transform.position;
                return;
            case State.Roam:
                if (!is_moving)
                {
                    StartCoroutine(randomMovement());
                    is_moving = true;
                }
                return;
        }
    }

    public Vector3 RandomNavMeshPosition(float radius)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius;
        randomDir += transform.position;
        NavMeshHit hit;
        Vector3 final_pos = Vector3.zero;
        if (NavMesh.SamplePosition(randomDir, out hit, radius, 1))
            final_pos = hit.position;
        Debug.Log(final_pos);
        return final_pos;
    }

    private IEnumerator randomMovement()
    {
        agent.destination = RandomNavMeshPosition(15);
        yield return new WaitForSeconds(5);
        is_moving = false;
    }
}
