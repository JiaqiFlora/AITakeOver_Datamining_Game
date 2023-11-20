using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private Vector3 originalPosition;
    private float pressDistance = 1.4f; 
    private float duration = 0.5f;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void pressButton(bool isHumanPress)
    {
        if(isHumanPress && GameManagement.instance.canHumanPress())
        {
            StartCoroutine(PressAndReleaseRoutine());
        }
        if(!isHumanPress && GameManagement.instance.canAIPress())
        {
            StartCoroutine(PressAndReleaseRoutine());
        }
    }

    IEnumerator PressAndReleaseRoutine()
    {
        GameManagement.instance.pressButtonAudio.Play();
        Vector3 targetPosition = originalPosition - transform.up * pressDistance;

        //yield return new WaitForSeconds(1);
        yield return MoveOverTime(targetPosition, duration / 2);
        yield return null;
        yield return MoveOverTime(originalPosition, duration / 2);
    }

    IEnumerator MoveOverTime(Vector3 target, float time)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.localPosition;

        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(startPosition, target, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = target;
    }

}
