using Pogo.Cosmetics;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.Challenges
{
    public class ChangeCosmeticScreenController : MonoBehaviour
    {
        public AppearanceScreenController Parent;
        public int GroupSize = 4;
        public GameObject CosmeticSelectButtonPrefab;
        public Transform CosmeticSelectGridRoot;

        private CosmeticSlotManifest currentManifest;
        private CosmeticDescriptor[] UnlockedItems;
        public CosmeticSlotManifest CurrentManifest
        {
            get { return currentManifest; }
            private set
            {
                currentManifest = value;
            }
        }

        public void Load(CosmeticSlotManifest manifest)
        {
            UnlockedItems = manifest.Items
                .Where(i => PogoGameManager.PogoInstance.CosmeticIsUnlocked(i))
                .ToArray();

            int groupCount = Mathf.CeilToInt(UnlockedItems.Length / (float)GroupSize);

            // remove excess buttons
            for (int n = CosmeticSelectGridRoot.childCount - 1; n >= groupCount * GroupSize; n--)
            {
                Destroy(CosmeticSelectGridRoot.GetChild(n).gameObject);
            }

            // intiialize all the buttons, creating if necessary
            for (int n = 0; n < groupCount * GroupSize; n++)
            {
                GameObject obj;
                if (n < CosmeticSelectGridRoot.childCount)
                {
                    obj = CosmeticSelectGridRoot.GetChild(n).gameObject;
                }
                else
                {
                    obj = Instantiate(CosmeticSelectButtonPrefab, CosmeticSelectGridRoot);

                    // we allocate a new int here because it's wrapped in as a closure D:
                    int index = n;
                    obj.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(index));
                }

                if (n < UnlockedItems.Length)
                {
                    obj.GetComponent<CosmeticSelectButtonController>().CurrentCosmetic = UnlockedItems[n];
                }
                else
                {
                    obj.GetComponent<CosmeticSelectButtonController>().CurrentCosmetic = null;
                }
            }
            CurrentManifest = manifest;
        }

        public void ButtonClicked(int buttonIndex)
        {
            if (buttonIndex < 0 ||
                buttonIndex >= UnlockedItems.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            PogoGameManager.PogoInstance.EquipCosmetic(UnlockedItems[buttonIndex]);
            Parent.Back();
        }
    }
}
