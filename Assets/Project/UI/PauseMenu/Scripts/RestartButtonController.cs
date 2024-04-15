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

    [HideInInspector]
    public int Clicks = 0;

    public int CustomMapRestartClicks = 2;
    public int FullGameRestartClicks = 3;

    public float ClickResetDelaySeconds = 5;


    public GameObject GlyphGameObject;
    public TextMeshProUGUI CountdownText;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(button_OnClick);
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
        Invalidate();
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
        Invalidate();
    }

    private void Invalidate()
    {
        CountdownText.text = (GetRestartClicks() - Clicks).ToString();
    }
}
