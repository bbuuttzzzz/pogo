using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("WizardUI/Tools/ColorCopier")]
public class ColorCopier : MonoBehaviour
{
    public Graphic ReadTarget;
    public Graphic WriteTarget;

    private void LateUpdate()
    {
        WriteTarget.color = ReadTarget.color;
    }
}
