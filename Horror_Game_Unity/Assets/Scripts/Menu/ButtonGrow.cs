using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect_transform;

    public Vector3 original_size = new Vector3(1f, 1f, 1f);
    public Vector3 new_size = new Vector3(1.2f, 1.2f, 1f);

    public float rect_multiplier = 1.3f;
    public Vector2 original_rect;
    public Vector2 new_rect;

    public AudioSource mouse_over_audio;

    void Start()
    {
        rect_transform = GetComponent<RectTransform>();
        rect_transform.localScale = original_size;
        original_rect = new Vector2(rect_transform.rect.width, rect_transform.rect.height);
        new_rect = new Vector2(rect_transform.rect.width * rect_multiplier, rect_transform.rect.height * rect_multiplier);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over_audio.Play();
        StopAllCoroutines();
        StartCoroutine(GrowDuration(original_size, new_size, original_rect, new_rect, 0.1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over_audio.Stop();
        StopAllCoroutines();
        StartCoroutine(GrowDuration(new_size, original_size, new_rect, original_rect, 0.1f));
    }

    private IEnumerator GrowDuration(Vector3 startG, Vector3 endG, Vector2 startW, Vector2 endW, float duration)
    {
        Debug.Log(rect_transform.localScale);
        float timer = 0;
        while (timer < duration)
        {
            rect_transform.localScale = Vector3.Lerp(startG, endG, timer/duration);
            rect_transform.sizeDelta = Vector2.Lerp(startW, endW, timer/duration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        rect_transform.localScale = endG;
        rect_transform.sizeDelta = endW;
    }
}
