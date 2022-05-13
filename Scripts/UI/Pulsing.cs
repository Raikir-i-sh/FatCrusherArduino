using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pulsing : MonoBehaviour
{
    [Range(0.2f, 1.4f)]
    public float approachSpeed = 0.5f;
    public float growthBound = 1.1f;
    public float shrinkBound = 0.9f;
    private float currentRatio = 1;
    public Image healthfillAmt;

    private IEnumerator coroutine;

    void Awake()
    {
        coroutine = pulse();
    }

    // Attach the coroutine
    void OnEnable()
    {
        StartCoroutine(coroutine);
    }

    void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    IEnumerator pulse()
    {
        // Run this indefinitely
        while (true)
        {
            approachSpeed = 0.2f + (1.4f - 0.2f) * (1 - healthfillAmt.fillAmount);

            // Get bigger for a few seconds
            while (this.currentRatio != this.growthBound)
            {
                // Determine the new ratio to use
                currentRatio = Mathf.MoveTowards(currentRatio, growthBound, approachSpeed * Time.deltaTime);

                transform.localScale = Vector3.one * currentRatio;

                yield return new WaitForEndOfFrame();
            }

            while (this.currentRatio != this.shrinkBound)
            {
                currentRatio = Mathf.MoveTowards(currentRatio, shrinkBound, approachSpeed * Time.deltaTime);

                transform.localScale = Vector3.one * currentRatio;

                yield return new WaitForEndOfFrame();
            }
        }
    }

}