using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveFileBoxController : MonoBehaviour
{
    public string DisplayName => $"Slot {SlotNumber}";
    public int SlotNumber;

    public TextMeshProUGUI TitleText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if (TitleText != null) TitleText.text = DisplayName;
    }
}
