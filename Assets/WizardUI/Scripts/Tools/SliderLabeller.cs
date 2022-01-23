using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderLabeller : MonoBehaviour
{
    public Text MinLabel;
    public Text MaxLabel;
    public Text DisplayLabel;

    Slider slider;

    private void OnValidate()
    {
        slider = GetComponent<Slider>();
        fixLabels();
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
        fixLabels();
        slider.onValueChanged.AddListener(onDisplayValueChanged);
    }

    private void onDisplayValueChanged(float displayValue)
    {
        DisplayLabel.text = displayValue.ToString();
    }

    private void fixLabels()
    {
        if (MinLabel != null)
        {
            MinLabel.text = slider.minValue.ToString();
        }
        if (MaxLabel != null)
        {
            MaxLabel.text = slider.maxValue.ToString();
        }
        if (DisplayLabel != null)
        {
            DisplayLabel.text = slider.value.ToString();
        }
    }
}
