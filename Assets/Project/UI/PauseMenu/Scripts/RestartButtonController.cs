using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUtils;

[RequireComponent(typeof(Button))]
public class RestartButtonController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnTrigger;
    private Button button;
    public Coroutine ResetCountdownCoroutine;

    private int Clicks = 0;

    public int CustomMapRestartClicks;
    public int FullGameRestartClicks;

    public float ClickResetDelaySeconds;


    public GameObject GlyphGameObject;
    public TextMeshProUGUI CountdownText;
    public TextMeshProUGUI LabelText;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(button_OnClick);
        GameManager.GameInstance.OnPauseStateChanged += OnPauseStateChanged;
        InvalidateUI();
    }
    protected void OnPauseStateChanged(object sender, bool nowPaused)
    {
        if (nowPaused)
        {
            InvalidateUI();
        }
    }

    private void button_OnClick()
    {
        if (ResetCountdownCoroutine != null) StopCoroutine(ResetCountdownCoroutine);
        Clicks++;

        if (Clicks >= GetRestartClicks())
        {
            OnTrigger?.Invoke();
            Clicks = 0;
        }
        else
        {
            StartCoroutine(ResetCountdownAsync());
        }
        InvalidateUI();
    }

    private int GetRestartClicks()
    {
        if (PogoGameManager.PogoInstance.CustomMapBuilder.CurrentCustomMap != null)
        {
            return CustomMapRestartClicks;
        }
        return FullGameRestartClicks;
    }

    private IEnumerator ResetCountdownAsync()
    {
        yield return new WaitForSecondsRealtime(ClickResetDelaySeconds);
        Clicks = 0;
        InvalidateUI();
    }

    private void InvalidateUI()
    {
        CountdownText.text = (GetRestartClicks() - Clicks).ToString();
        LabelText.text = PogoGameManager.PogoInstance.CustomMapBuilder.CurrentCustomMap != null
            ? "<b>RESTART</b>"
            : "<b>RESTART</b>\r\n<size=0.55em>Wipes Save Data!";
    }
}
