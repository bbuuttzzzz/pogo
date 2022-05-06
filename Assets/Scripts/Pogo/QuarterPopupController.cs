using UnityEngine;
using UnityEngine.UI;
using WizardUtils.Saving;

namespace Pogo
{
    public class QuarterPopupController : MonoBehaviour
    {
        public UnlockCounter QuarterCounter;
        public Text TitleText;
        public Text SubtitleText;

        private void Start()
        {
            int count = QuarterCounter.Check();
            int maxCount = QuarterCounter.Saves.Length;

            TitleText.text = $"Secret Quarter {count}/{maxCount}";
            if (count >= maxCount)
            {
                SubtitleText.text = "Now to find something to spend it on...";
            }
            else
            {
                SubtitleText.text = "Keep looking for more!";
            }
        }

    }
}
