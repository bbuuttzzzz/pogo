using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.Cosmetics
{
    public class VendingMachineButtonController : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float HighlightPower;

        public Color HighlightColor1;
        public Color HighlightColor2;

        public float HighlightWidth1;
        public float HighlightWidth2;

        public Outline outline;
        public Image CosmeticImage;
        
        public bool Highlighted;

        private bool buttonCanBeActive;
        public bool ButtonCanBeActive
        {
            get => buttonCanBeActive;
            set
            {
                buttonCanBeActive = value;
                UpdateDisplay();
            }
        }

        [NonSerialized]
        private VendingMachineUnlockData nextReward;
        public VendingMachineUnlockData NextReward
        {
            get => nextReward;
            set
            {
                nextReward = value;
                UpdateDisplay();
            }
        }
        public bool RewardAvailable => NextReward.CoinsNeeded <= 0;

        private void Awake()
        {
            UpdateDisplay();
        }

        private void LateUpdate()
        {
            if (Highlighted)
            {
                outline.OutlineWidth = Mathf.Lerp(HighlightWidth1, HighlightWidth2, HighlightPower);
                outline.OutlineColor = Color.Lerp(HighlightColor2 , HighlightColor1, HighlightPower);
            }
            else
            {
                outline.OutlineWidth = 0;
                outline.OutlineColor = Color.gray;
            }
        }

        [ContextMenu("Update Display Now")]
        public void UpdateDisplay()
        {
            GetComponent<Button>().interactable = ButtonCanBeActive && RewardAvailable;
            if (NextReward.Cosmetic != null)
            {
                CosmeticImage.enabled = true;
                CosmeticImage.sprite = NextReward.Cosmetic.Icon;
            }
            else
            {
                CosmeticImage.enabled = false;
            }
        }
    }
}
