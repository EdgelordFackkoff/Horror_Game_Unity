using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private float attack_damage;
    [SerializeField] protected bool isattacking = false;    
    [SerializeField] public bool can_attack = true;
    [SerializeField] protected Vector3 last_known_player_location;

    [Header("Audio")]
    [SerializeField] protected AudioSource move_source;
    [SerializeField] protected AudioClip[] move_sounds;
    [SerializeField] protected float move_sound_interval;
    [SerializeField] protected float next_step_time;
    [SerializeField] protected int last_played_move_sound = -1;
    [SerializeField] protected AudioSource chatter_source;
    [SerializeField] protected AudioClip[] chatter_sounds;
    [SerializeField] protected float chatter_mininterval;
    [SerializeField] protected float chatter_maxinterval;
    [SerializeField] protected float chatter_playchance;
    [SerializeField] protected float chatter_nextchattertime;
    [SerializeField] protected float chatter_last_played_chatter_sound = -1;

    [Header("Camera Effects")]
    [SerializeField] public Vector3 targetRotation;
    [SerializeField] public float attacked_move_duration;
    [SerializeField] public float attacked_shake_magnitude;
    [SerializeField] public float attacked_shake_frequency;

    [Header("References")] // Get exposure
    [SerializeField] public Level level;
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject damage_box;
    [SerializeField] public Transform eye_level;


    //For overrides
    public abstract void Start();
    public abstract void Handle_Animation();
    public abstract void Handle_Navigation();
    public abstract void Handle_Behaviour();
    public abstract void Handle_Audio();
    public abstract void Handle_Attack();
    public abstract void Handle_Misc();
    public abstract void Handle_Paused();

    //Update
    void Update()
    {
        if (level.paused)
        {
            Handle_Paused();
        }
        else
        {
            Handle_Behaviour();
            Handle_Animation();
            Handle_Navigation();
            Handle_Audio();
            Handle_Attack();
            Handle_Misc();
        }
    }

    //Grabs
    public float get_damage()
    {
        return attack_damage;
    }

    //States
    public void isAttacking_true()
    {
        isattacking = true;
    }
}
