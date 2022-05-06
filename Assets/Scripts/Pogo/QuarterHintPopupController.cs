using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WizardUtils.Saving;

namespace Pogo
{
    public class QuarterHintPopupController : MonoBehaviour
    {
        public UnlockCounter QuarterCounter;
        public Text SubtitleText;

        public string[] Messages;

        private void Start()
        {
            if (Messages.Length != QuarterCounter.Saves.Length)
            {
                throw new NotImplementedException("We don't have a message for each quarter");
            }

            for (int n = 0; n < QuarterCounter.Saves.Length; n++)
            {
                if (!QuarterCounter.Saves[n].IsUnlocked)
                {
                    SubtitleText.text = Messages[n];
                    break;
                }
            }
        }
    }
}
