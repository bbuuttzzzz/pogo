using System;
using UnityEngine;
using UnityEngine.UI;

namespace WizardUI
{
    [RequireComponent(typeof(Text))]
    public class Label : MonoBehaviour
    {
        Text textElement;

        private string text;
        public string Text { get => text; set => text = value; }

        public Color Color = Color.white;
        public Color PlaceholderColor = Color.white;
        public string PlaceholderText;

        bool shouldUsePlaceholder => Text != null && Text != "";

        private void Awake()
        {
            textElement = GetComponent<Text>();
        }

        private void OnValidate()
        {
            textElement = GetComponent<Text>();

            updateText();
        }

        private void updateText()
        {
            textElement.text = shouldUsePlaceholder ? Text : PlaceholderText;
            textElement.color = shouldUsePlaceholder ? Color : PlaceholderColor;
        }
    }
}
