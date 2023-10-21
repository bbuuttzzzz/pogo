using Pogo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardUtils;

[RequireComponent(typeof(ToggleableUIElement))]

public class StartChallengeMenuController : MonoBehaviour
{
    public UnityEvent OnCloseMenu;

    private void Awake()
    {
        var selfToggleable = GetComponent<ToggleableUIElement>();
        selfToggleable.OnOpen.AddListener(ShowChallengePreview);
        selfToggleable.OnClose.AddListener(HideChallengePreview);
    }

    public void CloseMenu() => OnCloseMenu?.Invoke();
    public void PlaceBalloon()
    {
        CloseMenu();
        PogoGameManager.PogoInstance.ChallengeBuilder.CalculateNewChallenge();
        PogoGameManager.PogoInstance.ChallengeBuilder.LoadChallenge();
    }

    private void ShowChallengePreview() => SetShowChallengePreview(true);
    private void HideChallengePreview() => SetShowChallengePreview(false);

    public void SetShowChallengePreview(bool state)
    {
        PogoGameManager.PogoInstance.ChallengeBuilder.SetShowChallengePreview(state);
    }
}
