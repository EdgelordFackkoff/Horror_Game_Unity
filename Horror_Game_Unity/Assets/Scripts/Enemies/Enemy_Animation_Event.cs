using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Animation_Event : MonoBehaviour
{
    [Header("Knight Animation Events")]
    //0 is inactive
    //1 is activating
    //2 is active
    //3 is deactivating
    //4 is moving, by definition 2 is already active
    private int knight_current_state = 0;

    //Knight
    public void Activating_Knight()
    {
        //Activating knight
        knight_current_state = 1;
        UnityEngine.Debug.Log(knight_current_state);

    }

    public void Activated_Knight()
    {
        //Activate knight
        knight_current_state = 2;
        UnityEngine.Debug.Log(knight_current_state);

    }

    public void Deactivating_Knight()
    {
        //Deactivating knight
        knight_current_state = 3;
        UnityEngine.Debug.Log(knight_current_state);

    }

    public void Deactivated_Knight()
    {
        //Deactivate knight
        knight_current_state = 0;
        UnityEngine.Debug.Log(knight_current_state);

    }

    public void Walking_Knight()
    {
        //Deactivate knight
        knight_current_state = 4;
        UnityEngine.Debug.Log(knight_current_state);

    }

    public int Knight_ActivationStatus()
    {
        //return
        return knight_current_state;
    }
}
