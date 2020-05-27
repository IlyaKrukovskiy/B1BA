using SimpleGames;
using System.Collections;
using UnityEngine;

public static class AnimationHelper 
{
    public delegate void MethodWithCoefficient(float coefficient);

    public static IEnumerator DoSomethingForSeconds(
        MethodWithCoefficient somethingMethod, 
        float seconds)
    {
        float currentTime = 0;
        float coefficient = 0;

        while (true)
        {
            currentTime += Time.deltaTime;
            coefficient = currentTime / seconds;

            if (currentTime > seconds)
            {
                somethingMethod(1);
                break;
            }

            somethingMethod(coefficient);

            yield return null;
        }
    }
}
