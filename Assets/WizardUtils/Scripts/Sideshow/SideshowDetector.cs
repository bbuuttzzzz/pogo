using Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardUtils
{
    [RequireComponent(typeof(PositionLookTarget))]
    [RequireComponent(typeof(Collider))]
    public class SideshowDetector : MonoBehaviour
    {
        private void Awake()
        {
            lookTarget = GetComponent<PositionLookTarget>();
        }

        public void OnTriggerStay(Collider other)
        {
            if(currentSideshow != null)
            {
                if (other == currentSideshow.Collider)
                {
                    lastDetectedTime = Time.time;
                }
                else
                {
                    Sideshow newSideshow = other.GetComponent<Sideshow>();
                    if (newSideshow != null && IsTriggeredBy(newSideshow))
                    {
                        if ((newSideshow.Target.position - transform.position).sqrMagnitude < (currentSideshow.Target.position - transform.position).sqrMagnitude)
                        {
                            currentSideshow = newSideshow;
                            lastDetectedTime = Time.time;
                        }
                    }
                }
            }
            else
            {
                Sideshow sideshow = other.GetComponent<Sideshow>();
                if (sideshow != null && IsTriggeredBy(sideshow))
                {
                    currentSideshow = sideshow;
                    lastDetectedTime = Time.time;
                }
            }
        }

        private void Update()
        {
            if (currentSideshow != null)
            {
                if (shouldLoseInterest)
                {
                    currentSideshow = null;
                    ResetLookTarget();
                }
                else
                {
                    SetLookTarget(currentSideshow.Target.transform.position);
                }
            }
        }

        private bool shouldLoseInterest => Time.time > lastDetectedTime + LoseInterestTime;

        #region Sideshow
        /// <summary>
        /// how long should the sideshow go undetected before we lose interest?
        /// </summary>
        public float LoseInterestTime = 0.2f;
        float lastDetectedTime;
        Sideshow currentSideshow;

        private bool IsTriggeredBy(Sideshow sideshow)
        {
            return true;
        }
        #endregion

        #region LookTarget
        PositionLookTarget lookTarget;

        void SetLookTarget(Vector3 worldPosition)
        {
            lookTarget.SetWorldPoint(worldPosition);
        }

        void ResetLookTarget()
        {
            lookTarget.ResetPoint();
        }
        #endregion
    }
}