using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pogo.CustomMaps.UI
{
    public class ChecklistEntry : MonoBehaviour
    {
        public TextMeshProUGUI TitleText;
        public Button HintButton;
        public Button AutoCompleteButton;
        public UnityEvent<ChecklistEntryData> OnShowHint;
        private ChecklistEntryData _Data;

        public ChecklistEntryData Data { get => _Data; private set => _Data = value; }

        private void Awake()
        {
            HintButton.onClick.AddListener(ShowHint);
            AutoCompleteButton.onClick.AddListener(AutoComplete);
        }

        public void SetComplete(bool isComplete)
        {
            _Data.IsComplete = isComplete;
            AutoCompleteButton.gameObject.SetActive(
                Data.AutoCompleteAction != null
                && (!Data.IsComplete || Data.AllowAutoCompleteWhenCompleted)
            );
        }

        public void Set(ChecklistEntryData data)
        {
            Data = data;

            TitleText.text = data.Title;
            HintButton.gameObject.SetActive(!string.IsNullOrEmpty(data.HintBody));
            SetComplete(data.IsComplete);
        }

        private void ShowHint()
        {
            OnShowHint?.Invoke(_Data);
        }

        private void AutoComplete()
        {
            Data.AutoCompleteAction?.Invoke();
        }

        #region Styles
        public enum Styles
        {
            Info,
            Warn,
            Error
        }

        public void SetStyle(Styles style)
        {

        }
        #endregion
    }
}
