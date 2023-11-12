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
    public class VendingUnlockPopupController : MonoBehaviour
    {
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

        public AnimationEventReplicator PopupEvent;

        public void TriggerUnlock()
        {
            PopupEvent.Call();
            
        }

        [ContextMenu("Update Display")]
        private void UpdateDisplay()
        {
            if (CurrentCosmetic == null)
            {
                IconImage.enabled = false;
            }
            else
            {
                IconImage.enabled = true;
                IconImage.sprite = CurrentCosmetic.Icon;

                // this lets us override LabelText by just unsetting the LabelText field
                if (LabelText != null) LabelText.text = CurrentCosmetic.DisplayName;
            }
        }
    }
}
