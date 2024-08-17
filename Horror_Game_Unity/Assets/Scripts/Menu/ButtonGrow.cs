using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public VerticalLayoutGroup panel;
    private RectTransform rect_transform;

    private Vector3 original_size;
    public Vector3 new_size = new Vector3(1.2f, 1.2f, 1f);

    private float original_spacing;
    private float offset = 30f;

    void Start()
    {
        rect_transform = GetComponent<RectTransform>();
        original_size = rect_transform.localScale;

        original_spacing = panel.spacing;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(GrowDuration(original_size, new_size, 0.1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(GrowDuration(new_size, original_size, 0.1f));
    }

    private IEnumerator GrowDuration(Vector3 startG, Vector3 endG, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            rect_transform.localScale = Vector3.Lerp(startG, endG, timer/duration);
            timer += Time.deltaTime;
            yield return null;
        }
        rect_transform.localScale = endG;
    }
}
