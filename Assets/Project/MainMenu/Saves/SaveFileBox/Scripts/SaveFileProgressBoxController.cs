using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileProgressBoxController : MonoBehaviour
{
    public enum States
    {
        Unfinished,
        Finished
    }

    public Image FillingImage;
    public Image OutlineImage;

    public Color FillingColor_Unfinished;
    public Color OutlineColor_Unfinished;

    public Color FillingColor_Finished;
    public Color OutlineColor_Finished;

    public void SetState(States state)
    {
        switch (state)
        {
            case States.Unfinished:
                FillingImage.color = FillingColor_Unfinished;
                OutlineImage.color = OutlineColor_Unfinished;
                break;
            case States.Finished:
                FillingImage.color = FillingColor_Finished;
                OutlineImage.color = OutlineColor_Finished;
                break;
        }
    }


}
