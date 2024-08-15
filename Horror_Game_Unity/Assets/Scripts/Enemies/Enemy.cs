using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public abstract class Enemy : MonoBehaviour
{
    [Header("Information")]
    [SerializeField] protected NavMeshAgent agent;

    //public enum Behaviour
    //{
    //    Librarian,
    //    Knight,
    //    Dancer
    //};
    //public Behaviour behaviour;

    [Header("References")] // Get exposure
    public Level level;
    public Animator animator;
    public Enemy_Animation_Event animation_events;

    //For overrides
    // Handle Animation
    public abstract void Handle_Animation();
    public abstract void Handle_Navigation();
    public abstract void Handle_Behaviour();
    public abstract void Handle_Misc();

    //Update
    void Update()
    {
        Handle_Animation();
        Handle_Navigation();
        Handle_Behaviour();
        Handle_Misc();
    }
}
