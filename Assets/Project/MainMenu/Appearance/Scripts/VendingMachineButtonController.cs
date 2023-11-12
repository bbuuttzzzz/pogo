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
        public Outline outline;

        private bool buttonCanBeActive;
        public bool ButtonCanBeActive
        {
            get => buttonCanBeActive;
            set
            {
                buttonCanBeActive = value;
                UpdateActive();
            }
        }

        [SerializeField]
        private bool rewardAvailable;
        public bool RewardAvailable
        {
            get => rewardAvailable;
            set
            {
                rewardAvailable = value;
                UpdateActive();
            }
        }

        public void UpdateActive()
        {
            GetComponent<Button>().interactable = ButtonCanBeActive && RewardAvailable;
        }

        public bool Highlighted;

        [Range(0f, 1f)]
        public float HighlightPower;

        public Color HighlightColor1;
        public Color HighlightColor2;

        public float HighlightWidth1;
        public float HighlightWidth2;

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
    }
}
