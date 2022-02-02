using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils
{
    public class PositionLookTarget : MonoBehaviour
    {
        [Tooltip("Vertical movement is multiplied by this scale")]
        public float YScale = 1f;

        public bool UseWorldSpace = false;

        Vector3 targetPosition;
        Vector3 homePosition;

        public Transform LookTarget;

        private void Awake()
        {
            homePosition = UseWorldSpace ? LookTarget.position : LookTarget.localPosition;
            targetPosition = homePosition;
        }

        public void Update()
        {
            if (UseWorldSpace)
            {
                LookTarget.position = targetPosition;
            }
            else
            {
                LookTarget.localPosition = targetPosition;
            }
        }

        public void SetWorldPoint(Vector3 newPosition)
        {
            targetPosition = UseWorldSpace ? newPosition
                : transform.InverseTransformPoint(newPosition);
        }

        public void SetLocalPoint(Vector3 newPosition)
        {
            targetPosition = UseWorldSpace ? transform.TransformPoint(newPosition)
                : newPosition;
        }

        public void ResetPoint()
        {
            targetPosition = homePosition;
        }

    }

}