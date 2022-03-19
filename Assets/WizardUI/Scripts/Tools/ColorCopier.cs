using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[AddComponentMenu("WizardUI/Tools/ColorCopier")]
public class ColorCopier : MonoBehaviour
{
    public Graphic ReadTarget;
    public Graphic WriteTarget;

    public bool AlphaOnly;


#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            updateColor();
        }
    }
#endif

    private void LateUpdate()
    {
        updateColor();
    }

    private void updateColor()
    {
        if (WriteTarget != null && ReadTarget != null)
        {
            if (AlphaOnly)
            {
                Color newColor = WriteTarget.color;
                newColor.a = ReadTarget.color.a;
                WriteTarget.color = newColor;
            }
            else
            {
                WriteTarget.color = ReadTarget.color;
            }
        }
    }
    }
