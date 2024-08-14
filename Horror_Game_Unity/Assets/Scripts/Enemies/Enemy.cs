using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
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

    [Header("Level")] // Get exposure
    public GameObject level_gameobject;
    public Level level;

}
