using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardEffects
{
    public class DecalController : MonoBehaviour
    {
        public void PerformEffect(EffectEventArgs e)
        {
            transform.position = e.EffectData.position;
            float angle = Mathf.Floor(Random.value * 4) % 4 * 90;
            transform.rotation = Quaternion.LookRotation(e.EffectData.normal * -1, Vector3.up) * Quaternion.Euler(0, 0, angle);
        }
    }
}