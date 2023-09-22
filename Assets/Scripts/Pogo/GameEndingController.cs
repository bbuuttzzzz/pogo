using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;
using WizardUtils.SceneManagement;

namespace Assets.Scripts.Pogo
{
    public class GameEndingController : MonoBehaviour
    {
        public float LevelChangeDelay;

        public ControlSceneDescriptor CreditsScene;
        public UnityEvent OnStoreFinalStats;

        public void StoreFinalStats()
        {
            PogoGameManager.PogoInstance.OnStoreFinalStats.Invoke();
            OnStoreFinalStats.Invoke();
        }

        public void StartEndGame()
        {
            PogoGameManager.PogoInstance.FinishChapter();
            PogoGameManager.PogoInstance.SaveSlot();
            StartCoroutine(ChangeLevelDelayed(LevelChangeDelay));
        }

        private IEnumerator ChangeLevelDelayed(float levelChangeDelay)
        {
            yield return new WaitForSecondsRealtime(levelChangeDelay);

            GameManager.GameInstance.LoadControlScene(CreditsScene);
        }
    }
}
