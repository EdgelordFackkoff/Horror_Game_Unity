using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public abstract class Enemy : MonoBehaviour
{
    [Header("Information")]
    [SerializeField] protected NavMeshAgent agent;

    [Header("References")] // Get exposure
    public Level level;
    public Animator animator;


    //For overrides
    // Handle Animation
    public abstract void Handle_Animation();
    public abstract void Handle_Navigation();
    public abstract void Handle_Behaviour();
    public abstract void Handle_Misc();

    //Update
    void Update()
    {
        Handle_Behaviour();
        Handle_Animation();
        Handle_Navigation();
        Handle_Misc();
    }
}
