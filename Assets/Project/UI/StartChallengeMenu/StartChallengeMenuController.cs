using Pogo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartChallengeMenuController : MonoBehaviour
{
    public UnityEvent OnCloseMenu;

    public void CloseMenu() => OnCloseMenu?.Invoke();
    public void PlaceBalloon()
    {
        CloseMenu();
        PogoGameManager.PogoInstance.ChallengeBuilder.CalculateNewChallenge();
        PogoGameManager.PogoInstance.ChallengeBuilder.LoadChallenge();
    }

    public void SetShowChallengePreview(bool state)
    {
        PogoGameManager.PogoInstance.ChallengeBuilder.SetShowChallengePreview(state);
    }
}
