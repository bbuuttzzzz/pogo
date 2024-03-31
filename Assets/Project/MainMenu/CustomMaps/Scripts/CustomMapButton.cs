using Pogo.CustomMaps.Indexing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.CustomMaps.UI
{
    public class CustomMapButton : MonoBehaviour
    {
        public Button UIButton;
        public Sprite DefaultPreviewSprite;
        public Image PreviewImage;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI AuthorText;
        public TextMeshProUGUI VersionText;

        public MapHeader Header
        {
            get => _Header;
            set
            {
                _Header = value;
                HeaderChanged();
            }
        }
        private MapHeader _Header;

        public void HeaderChanged()
        {
            PreviewImage.sprite = _Header.PreviewSprite != null ? _Header.PreviewSprite : DefaultPreviewSprite;
            TitleText.text = _Header.DisplayName ?? _Header.MapName ?? "Unknown";
            AuthorText.text = _Header.AuthorName ?? "Anonymous";
            VersionText.text = $"v{_Header.Version ?? "0.1.0"}";
        }
    }
}