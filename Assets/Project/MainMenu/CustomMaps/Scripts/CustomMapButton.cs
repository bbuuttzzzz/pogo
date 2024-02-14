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
        public Sprite DefaultPreviewSprite;
        public Image PreviewImage;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI AuthorText;
        public TextMeshProUGUI VersionText;

        public void SetFromHeader(MapHeader header)
        {
            PreviewImage.sprite = header.PreviewSprite != null ? header.PreviewSprite : DefaultPreviewSprite;
            TitleText.text = header.MapName ?? "Unknown";
            AuthorText.text = header.AuthorName ?? "Anonymous";
            VersionText.text = $"v{header.Version ?? "0.1.0"}";
        }
    }
}