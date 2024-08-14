using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Animation_Event : MonoBehaviour
{
    [Header("Knight Animation Events")]
    private bool isactive = false;

    //Knight
    public void Activated()
    {
        //Active knight
        isactive = true;
    }

    public void Deactivated()
    {
        //Active knight
        isactive = false;
    }

    public bool ActivationStatus()
    {
        //return
        return isactive;
    }
}
