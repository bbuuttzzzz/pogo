using System;
using TMPro;
using UnityEditor.Timeline.Actions;
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
            bool showAutoCompleteButton = Data.AutoCompleteMode switch
            {
                ChecklistEntryData.AutoCompleteModes.Hide => false,
                ChecklistEntryData.AutoCompleteModes.Show => true,
                ChecklistEntryData.AutoCompleteModes.ShowWhenComplete => status.IsComplete,
                ChecklistEntryData.AutoCompleteModes.ShowWhenIncomplete => !status.IsComplete,
                _ => throw new NotImplementedException()
            };
            Styles style = status.IsComplete
                ? Styles.Complete
                : _Data.IsRequired
                    ? Styles.IncompleteRequired
                    : Styles.IncompleteOptional;

            _Data.Status = status;
            ValueText.text = status.Value ?? _Data.DefaultDisplayValue;
            AutoCompleteButton.gameObject.SetActive(showAutoCompleteButton);
            SetStyle(style);
        }

        [ContextMenu("Style Complete")]
        public void StyleComplete() => SetStyle(Styles.Complete);
        [ContextMenu("Style IncompleteRequired")]
        public void StyleIncompleteRequired() => SetStyle(Styles.IncompleteRequired);
        [ContextMenu("Style IncompleteOptional")]
        public void StyleIncompleteOptional() => SetStyle(Styles.IncompleteOptional);

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
