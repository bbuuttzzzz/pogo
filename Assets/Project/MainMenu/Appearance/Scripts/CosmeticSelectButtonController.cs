using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.Cosmetics
{
    [RequireComponent(typeof(Button))]
    public class CosmeticSelectButtonController : MonoBehaviour
    {
        public Button Button => GetComponent<Button>();

        public CosmeticDescriptor CurrentCosmetic
        {
            get => currentCosmetic;
            set
            {
                if (currentCosmetic != value)
                {
                    currentCosmetic = value;
                    UpdateDisplay();
                }
            }
        }

        [SerializeField]
        private CosmeticDescriptor currentCosmetic;
        [SerializeField]
        private Image IconImage;
        [SerializeField]
        private TextMeshProUGUI LabelText;

        [ContextMenu("Update Display")]
        private void UpdateDisplay()
        {
            if (CurrentCosmetic == null)
            {
                IconImage.enabled = false;
                Button.interactable = false;
            }
            else
            {
                Button.interactable = true;
                IconImage.enabled = true;
                IconImage.sprite = CurrentCosmetic.Icon;

                // this lets us override LabelText by just unsetting the LabelText field
                if (LabelText != null) LabelText.text = CurrentCosmetic.DisplayName;
            }
        }
    }
}
