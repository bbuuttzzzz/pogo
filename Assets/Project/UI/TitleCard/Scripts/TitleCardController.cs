using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TitleCardController : MonoBehaviour
{
    public float TypeDelaySeconds;
    public float DestroyDelaySeconds = 5f;
    public AudioSource AudioSource;
    public float MinimumPitch = 1;
    public float MaximumPitch = 1;

    public void DisplayTitle(string title, float delay = 0)
    {
        StartCoroutine(SetTextAnimated(title, delay));
        Destroy(gameObject, delay + DestroyDelaySeconds);
    }



    private IEnumerator SetTextAnimated(string title, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        var text = GetComponent<TextMeshProUGUI>();
        GetComponent<Animator>().SetTrigger("Display");
        Vector2 desiredSize = text.GetPreferredValues(title);
        GetComponent<RectTransform>().sizeDelta = desiredSize;

        for (int n = 0; n <= title.Length; n++)
        {
            text.text = title.Substring(0,n);
            AudioSource.pitch = Random.Range(MinimumPitch,MaximumPitch);
            AudioSource.Play();
            yield return new WaitForSecondsRealtime(TypeDelaySeconds);
        }
    }
}
