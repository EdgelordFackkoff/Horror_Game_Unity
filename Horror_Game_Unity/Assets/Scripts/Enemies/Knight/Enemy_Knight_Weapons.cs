using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Knight_Weapons : MonoBehaviour
{

    //Polearm_Arrays and Prefabs
    [Header("Info")]
    [SerializeField] public GameObject slot;
    [SerializeField] private int polearm_int;
    [SerializeField] private GameObject[] polearms_list;
    [SerializeField] public AudioSource polearm_hit;
    [SerializeField] private AudioClip[] polearms_sounds;


    // Start is called before the first frame update
    void Start()
    {
        //Random Integer
        int random = UnityEngine.Random.Range(0, polearms_list.Length);
        UnityEngine.Debug.Log(random);
        //Assign the int
        polearm_int = random;
        //Go through list and only activate one
        for (int i = 0; polearms_list.Length > i; i++)
        {
            if (i != polearm_int)
            {
                //Deactivate
                polearms_list[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
