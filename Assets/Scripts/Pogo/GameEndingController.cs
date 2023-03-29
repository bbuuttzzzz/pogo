using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WizardUtils;
using WizardUtils.SceneManagement;

namespace Assets.Scripts.Pogo
{
    public class GameEndingController : MonoBehaviour
    {
        public float LevelChangeDelay;

        public ControlSceneDescriptor CreditsScene;

        public void StartEndGame()
        {
            StartCoroutine(ChangeLevelDelayed(LevelChangeDelay));
        }

        private IEnumerator ChangeLevelDelayed(float levelChangeDelay)
        {
            yield return new WaitForSecondsRealtime(levelChangeDelay);

            GameManager.GameInstance.LoadControlScene(CreditsScene);
            PogoGameManager.PogoInstance.OnStoreFinalStats.Invoke();
        }
    }
}
