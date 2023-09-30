using Pogo;
using UnityEngine;
using UnityEngine.UI;
using WizardUtils;

public class AssistModeMenuController : MonoBehaviour
{
    public GameObject AssistModeRoot;
    public Button SkipButton;
    public PauseMenuController pauseMenuController;

    public void Refresh()
    {
        if (PogoGameManager.PogoInstance.CurrentDifficulty != PogoGameManager.Difficulty.Freeplay)
        {
            AssistModeRoot.SetActive(false);
            return;
        }
        AssistModeRoot.SetActive(true);

        SkipButton.interactable = PogoGameManager.PogoInstance.CanSkipCheckpoint();
    }

    public void Skip()
    {
        if (PogoGameManager.PogoInstance.CurrentDifficulty != PogoGameManager.Difficulty.Freeplay)
        {
            Debug.LogError($"Skip button was pressed outside of Assist Mode???");
            AssistModeRoot.SetActive(false);
            return;
        }
        if (!PogoGameManager.PogoInstance.TrySkipCheckpoint())
        {
            Debug.LogError($"Failed to skip CurrentCheckpoint: {PogoGameManager.PogoInstance.CurrentCheckpoint.Descriptor}");
            SkipButton.interactable = false;
            return;
        }

        pauseMenuController.Resume();
    }
}