using UnityEngine;
using UnityEngine.UI;
using WizardUI;
using WizardUtils;

namespace Pogo
{
    public class ScoreBoardController : MonoBehaviour
    {
        public TimeFormatter TimeSetter;
        public IntegerFormatter DeathsSetter;
        public IntegerFormatter SecretsSetter;

        public ChapterDescriptor HideChapterNameIfItsThis;
        public GameObject StartPointObject;
        public Text ChapterNameText;

        public void Read()
        {
            TimeSetter.FormatFloat(Time.time - PogoGameManager.PogoInstance.GameStartTime);
            DeathsSetter.FormatInt(PogoGameManager.PogoInstance.NumberOfDeaths);
            SecretsSetter.FormatInt(PogoGameManager.PogoInstance.SecretsFoundCount);
            ChapterDescriptor startingChapter = PogoGameManager.PogoInstance.StartingChapter;
            if (startingChapter == HideChapterNameIfItsThis)
            {
                StartPointObject.SetActive(false);
            }
            else
            {
                StartPointObject.SetActive(true);
                ChapterNameText.text = startingChapter?.Title?? "Nowhere???";
            }
        }
    }
}
