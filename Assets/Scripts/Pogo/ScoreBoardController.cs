using UnityEngine;
using WizardUI;
using WizardUtils;

namespace Pogo
{
    public class ScoreBoardController : MonoBehaviour
    {
        public TimeFormatter TimeSetter;
        public IntegerFormatter DeathsSetter;
        public IntegerFormatter SecretsSetter;

        public void Read()
        {
            TimeSetter.FormatFloat(Time.time - PogoGameManager.PogoInstance.GameStartTime);
            DeathsSetter.FormatInt(PogoGameManager.PogoInstance.NumberOfDeaths);
            SecretsSetter.FormatInt(PogoGameManager.PogoInstance.SecretsFoundCount);
        }
    }
}
