using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutGroup))]
public class TextValueFeed : MonoBehaviour
{
    public GameObject TextPrefab;
    public Queue<Text> textQueue;
    public int MaxFeedCount;

    public Gradient ColorMap;
    public bool LoopMap;
    public float MaxColorValue;

    private void Awake()
    {
        textQueue = new Queue<Text>();
    }

    public void AddValue(float value)
    {
        Text text = nextText();
        text.text = value.ToString("N2");
        text.color = sampleColor(value);
    }

    Text nextText()
    {
        if (textQueue.Count < MaxFeedCount)
        {
            var textObject = Instantiate(TextPrefab, transform);
            textObject.transform.SetSiblingIndex(0);
            Text text = textObject.GetComponent<Text>();
            textQueue.Enqueue(text);
            return text;
        }
        else
        {
            Text text = textQueue.Dequeue();
            text.transform.SetSiblingIndex(0);
            textQueue.Enqueue(text);
            return text;
        }

    }

    Color sampleColor(float value)
    {
        float time = LoopMap ? (value % MaxColorValue) / MaxColorValue
            : Mathf.Min(value / MaxColorValue, 1);

        return ColorMap.Evaluate(time);
    }
}
