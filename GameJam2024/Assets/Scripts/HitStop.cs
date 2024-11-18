using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    private bool wait;

    public void TimeStop(float time)
    {
        if (wait)
        {
            return;
        }
        Time.timeScale = 0.0f;
        StartCoroutine(Wait(time));
    }

    IEnumerator Wait(float time)
    {
        wait = true;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1.0f;
        wait = false;
    }
}
