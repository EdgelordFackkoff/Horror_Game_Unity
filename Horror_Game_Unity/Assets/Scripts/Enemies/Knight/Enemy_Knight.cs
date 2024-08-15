using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Enemy_Knight : Enemy
{
    //Knight unique booleans
    [Header("Unique")]
    //0 is inactive
    //1 is active
    [SerializeField] private int current_active_status = 0;

    // Start
    void Start()
    {
        
    }

    //Activate or Deactivae Knight
    public void activate_knight(int x)
    {
        if (x <= 4 && x >= 0)
        {
            current_active_status = x;
        }
    }

    //Handle animation
    public override void Handle_Animation()
    {
        //See current animation
        AnimatorStateInfo anim_info = animator.GetCurrentAnimatorStateInfo(0);
        

        //Go through evet statuses
        if (current_active_status == 0)
        {
            //If already playing
            if (!anim_info.IsName("Inactive_idle"))
            {
                //See if deactivation animation is playing
                if (anim_info.IsName("Become_InActive"))
                {
                    //Check if finished
                    if (anim_info.normalizedTime >= 1.0f)
                    {
                        //If  finished switch around
                        animator.SetBool("Inactive", true);
                        animator.SetBool("Becoming_InActive", false);
                        UnityEngine.Debug.Log("Deactivated");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Still Deactivating");
                    }
                }

                //Else change to deactivation
                else
                {
                    UnityEngine.Debug.Log("Deactivating");
                    animator.SetBool("Walking", false);
                    animator.SetBool("Active", false);
                    animator.SetBool("Becoming_InActive", true);
                }
            }

        }

        if (current_active_status == 1)
        {
            if (!anim_info.IsName("Active_Idle"))
            {
                //See if activating animation is playing
                if (anim_info.IsName("Become_Active"))
                {
                    //Check if finished
                    if (anim_info.normalizedTime >= 1.0f)
                    {
                        //If finished switch around
                        animator.SetBool("Inactive", false);
                        animator.SetBool("Active", true);
                        animator.SetBool("Becoming_Active", false);
                        UnityEngine.Debug.Log("Activated");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Still Activating");
                    }
                }
            }
            //Else change to activation
            else
            {
                animator.SetBool("Inactive", false);
                animator.SetBool("Becoming_Active", true);
                UnityEngine.Debug.Log("Becoming Activated");
            }

        }
    }

    //Handle nvigation
    public override void Handle_Navigation()
    {

    }

    //Handle animation
    public override void Handle_Behaviour()
    {
        //Some changes to test
        if (level.exposure_level >= 1 && current_active_status == 0)
        {
            current_active_status = 1;
        }

        if (level.exposure_level < 1 && current_active_status != 0)
        {
            current_active_status = 0;
        }

    }

    //Handle 
    public override void Handle_Misc()
    {

    }
}
