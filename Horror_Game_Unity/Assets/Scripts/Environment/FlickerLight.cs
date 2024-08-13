using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [Header("Light")]
    private Light my_light;
    [SerializeField] private float interval = 1;

    [Header("Timing")]
    [SerializeField] private float lowest_interval = 0.1f;
    [SerializeField] private float highest_interval = 1;

    private AudioSource flicker_audio;

    private float timer;

    void Start()
    {
        // Auto sets to object's components
        my_light = GetComponent<Light>();
        flicker_audio = GetComponent<AudioSource>();
    }

    void Update() // If script is too expensive can add to occlusion culling later
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            my_light.enabled = !my_light.enabled;
            interval = Random.Range(lowest_interval, highest_interval);
            timer = 0;
        }
    }
}
