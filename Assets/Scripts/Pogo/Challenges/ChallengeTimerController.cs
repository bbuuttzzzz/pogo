using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUI;

namespace Pogo.Challenges
{
    [RequireComponent(typeof(StringStopWatch))]
    public class ChallengeTimerController : MonoBehaviour
    {
        public ChallengeBuilder challengeBuilder;
        Animator animator;
        StringStopWatch stopWatch;
        public Text popupText;

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                animator.SetBool("shouldShow", value);
                if (!isActive)
                {
                    StopTimer();
                }
                isActive = value;
            }
        }

        private void Awake()
        {
            stopWatch = GetComponent<StringStopWatch>();
            animator = GetComponent<Animator>();
            challengeBuilder.OnChallengeChanged.AddListener(onChallengeChanged);
            challengeBuilder.OnChallengeComplete.AddListener(onChallengeComplete);
            challengeBuilder.OnChallengeReset.AddListener(onChallengeReset);
        }

        private void onChallengeReset()
        {
            ResetTimer();
        }

        private void onChallengeChanged(Challenge challenge)
        {
            IsActive = challenge != null;
        }

        float cachedBestTime;
        private void onChallengeComplete()
        {
            StopTimer();
            if (challengeBuilder.CurrentChallenge.BestTimeMS != ushort.MaxValue)
            {
                spawnFloater(challengeBuilder.CurrentChallenge.LastAttemptTime - cachedBestTime);
            }
            cachedBestTime = challengeBuilder.CurrentChallenge.BestTime;
        }

        public void ResetTimer()
        {
            stopWatch.Set();
            stopWatch.CalculateOnUpdate = true;
        }
        public void StopTimer()
        {
            stopWatch.CalculateOnUpdate = false;
            if (challengeBuilder.CurrentChallenge != null)
            {
                stopWatch.SetToValue(challengeBuilder.CurrentChallenge.LastAttemptTime);
            }
        }

        void spawnFloater(float value)
        {
            string textValue = (value < 0 ? "" : "+") + value.ToString("N3");

            popupText.text = textValue;
            animator.SetFloat("delta", value);
            animator.SetTrigger("openPopup");
            Debug.Log("Spawned floater " + textValue);
        }
    }
}
