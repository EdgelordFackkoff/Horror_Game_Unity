using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerManager : MonoBehaviour
{
    [Header("Information")]
    public float music_interval_min;
    public float music_interval_max;
    public float music_interval_current;

    public bool is_music_playing = false;

    private void Start()
    {
        music_interval_min = 5;
        music_interval_max = 15;

        StartCoroutine(MusicManager());
    }

    private IEnumerator MusicManager()
    {
        while (true)
        {
            music_interval_current = Random.Range(music_interval_min, music_interval_max);
            yield return new WaitForSeconds(music_interval_current);

            if (is_music_playing)
            {
                is_music_playing = false;
            }
            else if (!is_music_playing)
            {
                is_music_playing = true;
            }
        }
    }
}
