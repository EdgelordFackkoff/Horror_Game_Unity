using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pressure_Plate : MonoBehaviour
{
    [Header("Unique")]
    [SerializeField] public bool is_activated = false;
    [SerializeField] public bool job_done = false;
    [SerializeField] private AudioSource plate_audio;
    [SerializeField] public float loweredAmount = 0.05f;
    [SerializeField] private Vector3 originalPosition;

    void Start()
    {
        // Store the original position of the pressure plate
        originalPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer == 3 || other.gameObject.layer == 15) && !is_activated && !job_done)
        {
            is_activated = true;
            //Play Audio
            plate_audio.Play();
            LowerPressurePlate();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.layer == 3 || other.gameObject.layer == 15) && !is_activated && !job_done)
        {
            is_activated = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.layer == 3 || other.gameObject.layer == 15) && is_activated && !job_done)
        {
            is_activated = false;
            //Play Audio
            plate_audio.Play();
            RaisePressurePlate();
        }
    }

    private void LowerPressurePlate()
    {
        transform.position = originalPosition - new Vector3(0, loweredAmount, 0);
    }

    private void RaisePressurePlate()
    {
        transform.position = originalPosition;
    }

    public void jobs_done()
    {
        job_done = true;
        LowerPressurePlate();
    }
}
