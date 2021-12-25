using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] private float duration_s = 60f;
    private Coroutine activeCoroutine = null;
    
    IEnumerator AutoScroll(
        ScrollRect scrollRect,
        float duration)
    {
        yield return new WaitForSeconds(2f);
        float tempSpeed = 1 / duration;
        while (gameObject.activeSelf)
        {
            scrollRect.verticalNormalizedPosition -= (Time.deltaTime * tempSpeed);
            yield return null;
        }
    }

    public void OnClickCredits()
    {
        if(activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        scrollRect.verticalNormalizedPosition = 1;
        activeCoroutine = StartCoroutine(AutoScroll(scrollRect, duration_s));
    }

 
}
