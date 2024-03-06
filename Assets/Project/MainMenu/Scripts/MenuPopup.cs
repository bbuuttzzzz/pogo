using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Pogo.MainMenu
{
    public class MenuPopup : MonoBehaviour
    {
        public PogoMainMenuController parent;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI BodyText;
        public Button OKButton;
        public TextMeshProUGUI OKButtonText;
        public Button CancelButton;
        public TextMeshProUGUI CancelButtonText;

        private Action OkPressedCallback;
        private Action CancelPressedCallback;

        private void Awake()
        {
            OKButton.onClick.AddListener(OKButton_OnClick);
            CancelButton.onClick.AddListener(CancelButton_OnClick);
        }

        private void OKButton_OnClick()
        {
            OkPressedCallback?.Invoke();
            parent.CloseOptionsScreen();
        }
        private void CancelButton_OnClick()
        {
            CancelPressedCallback?.Invoke();
            parent.CloseOptionsScreen();
        }

        public void Set(MenuPopupData data)
        {
            TitleText.text = data.Title;
            BodyText.text = data.Body;
            OKButton.gameObject.SetActive(data.ShowOkButton);
            OKButtonText.text = data.OkText;
            OkPressedCallback = data.OkPressedCallback;
            CancelButton.gameObject.SetActive(data.ShowCancelButton);
            CancelButtonText.text = data.CancelText;
            CancelPressedCallback = data.CancelPressedCallback;
        }
    }
}
