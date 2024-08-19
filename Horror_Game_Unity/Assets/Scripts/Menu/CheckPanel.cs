using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public VerticalLayoutGroup panel;

    public float original_spacing;
    public float offset = 30f;

    void Start()
    {
        panel = GetComponent<VerticalLayoutGroup>();
        original_spacing = panel.spacing;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(GrowDuration(original_spacing, original_spacing + offset, 0.1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(GrowDuration(original_spacing + offset, original_spacing, 0.1f));
    }

    private IEnumerator GrowDuration(float startS, float endS, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            panel.spacing = Mathf.Lerp(startS, endS, timer / duration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        panel.spacing = endS;
    }
}
