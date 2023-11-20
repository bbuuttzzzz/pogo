using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pogo.AssemblyLines
{
    public class AssemblyLineController : MonoBehaviour
    {
        public AnimationClip TargetAnimationClip;
        public GameObject Prefab;
        public float DelaySeconds;
        public float CycleOffset;
    }
}