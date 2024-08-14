using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Knight : Enemy
{
    //Knight unique booleans
    [Header("Unique")]
    [SerializeField] public bool active = false;

    // Start
    void Start()
    {
        animator.SetBool("Inactive", true);
    }

    // Animation Tests
    void Update()
    {
        if (level.exposure_level > 0)
        {
            //It becomes active
            animator.SetBool("Inactive", false);
            animator.SetBool("Active", true);

        }

    }
}
