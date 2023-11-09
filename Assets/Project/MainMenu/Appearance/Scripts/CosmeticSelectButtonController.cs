using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                currentCosmetic = value;
                UpdateDisplay();
            }
        }

        [SerializeField]
        private CosmeticDescriptor currentCosmetic;
        public Image IconImage;

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
            }
        }
    }
}
