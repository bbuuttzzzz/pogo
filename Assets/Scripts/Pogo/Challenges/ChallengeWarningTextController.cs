using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.Challenges
{

    public class ChallengeWarningTextController : MonoBehaviour
    {
        ChallengeBuilder challengeBuilder;
        Text text;

        private void Start()
        {
            challengeBuilder = PogoGameManager.GameInstance.GetComponent<ChallengeBuilder>();
            challengeBuilder.OnDecodeFailed.AddListener(ReceiveLoadError);
            text = GetComponent<Text>();
        }

        public void ReceiveLoadError(ChallengeBuilder.DecodeFailReason failReason)
        {
            if (failReason == ChallengeBuilder.DecodeFailReason.WrongLength)
            {
                setText($"Correct codes will be {ChallengeBuilder.PayloadLength} characters long", Color.red);
            }
            else if (failReason == ChallengeBuilder.DecodeFailReason.Invalid)
            {
                setText($"Failed to decode. Is this code correct?", Color.red);
            }
        }

        Coroutine resetRoutine;
        void setText(string text, Color color)
        {
            if (this.text == null) this.text = GetComponent<Text>();

            this.text.text = text;
            this.text.color = color;

            if (resetRoutine != null) StopCoroutine(resetRoutine);
            resetRoutine = StartCoroutine(ResetAfterSeconds(3));
        }

        IEnumerator ResetAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            resetRoutine = null;
            ResetText();
        }

        public void ResetText()
        {
            if (this.text == null) this.text = GetComponent<Text>();
            if (challengeBuilder == null) this.challengeBuilder = PogoGameManager.PogoInstance.GetComponent<ChallengeBuilder>();

            if (challengeBuilder.CurrentChallenge != null && challengeBuilder.CurrentChallenge.BestTimeMS > 60_000)
            {
                this.text.text = "To share this challenge, complete it in less than 60 seconds";
            }
            else
            {
                this.text.text = "";
            }
            this.text.color = Color.gray;
        }
    }
}
