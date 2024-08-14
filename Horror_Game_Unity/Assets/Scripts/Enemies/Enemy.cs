using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Enemy : MonoBehaviour
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
}
