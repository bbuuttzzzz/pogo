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
        public TextMeshProUGUI ValueText;
        public Image BodyImage;
        public Button HintButton;
        public Button AutoCompleteButton;
        public UnityEvent<ChecklistEntryData> OnShowHint;
        private ChecklistEntryData _Data;

        [Tooltip("Complete / IncompleteOptional / IncompleteRequired")]
        public StyleData[] StyleDatas = new StyleData[3];

        public ChecklistEntryData Data { get => _Data; private set => _Data = value; }

        private void Awake()
        {
            HintButton.onClick.AddListener(ShowHint);
            AutoCompleteButton.onClick.AddListener(AutoComplete);
        }

        public void SetStatus(ChecklistEntryStatus status)
        {
            _Data.Status = status;
            ValueText.text = status.Value ?? "";
            AutoCompleteButton.gameObject.SetActive(
                Data.AutoCompleteAction != null
                && (!Data.Status.IsComplete || Data.AllowAutoCompleteWhenCompleted)
            );
        }

        public void Set(ChecklistEntryData data)
        {
            Data = data;

            TitleText.text = data.Title;
            HintButton.gameObject.SetActive(!string.IsNullOrEmpty(data.HintBody));
            SetStatus(data.Status);
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
            Complete,
            IncompleteOptional,
            IncompleteRequired
        }

        [System.Serializable]
        public struct StyleData
        {
            public Color BodyImageColor;
        }

        public void SetStyle(Styles style)
        {
            StyleData data = StyleDatas[(int)style];
            BodyImage.color = data.BodyImageColor;
        }
        #endregion
    }
}
