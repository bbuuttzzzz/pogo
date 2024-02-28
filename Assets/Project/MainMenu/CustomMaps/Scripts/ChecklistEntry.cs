using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.CustomMaps.UI
{
    public class ChecklistEntry : MonoBehaviour
    {
        public TextMeshProUGUI TitleText;
        public Button HintButton;
        public Button AutoCompleteButton;
        private Action<string> ShowHintAction;

        private ChecklistEntryData Data;

        private void Awake()
        {
            HintButton.onClick.AddListener(ShowHint);
            AutoCompleteButton.onClick.AddListener(AutoComplete);
        }

        public void Set(ChecklistEntryData data, Action<string> showHintAction)
        {
            Data = data;
            ShowHintAction = showHintAction;

            TitleText.text = data.Title;
            HintButton.gameObject.SetActive(!string.IsNullOrEmpty(data.HintBody));
            AutoCompleteButton.gameObject.SetActive(
                Data.AutoCompleteAction != null
                && (!Data.IsComplete || Data.AllowAutoCompleteWhenCompleted)
            );
        }

        private void ShowHint()
        {
            ShowHintAction?.Invoke(Data.HintBody);
        }

        private void AutoComplete()
        {
            Data.AutoCompleteAction?.Invoke();
        }
    }
}
