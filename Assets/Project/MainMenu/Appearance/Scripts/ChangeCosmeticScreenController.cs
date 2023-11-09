using Pogo.Cosmetics;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.Challenges
{
    public class ChangeCosmeticScreenController : MonoBehaviour
    {
        public int GroupSize = 4;
        public GameObject CosmeticSelectButtonPrefab;
        public Transform CosmeticSelectGridRoot;
        public CosmeticManifest CurrentManifest { get; private set; }

        public void Load(CosmeticManifest manifest)
        {
            int groupCount = Mathf.CeilToInt(manifest.Items.Length / (float)GroupSize);

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
                    obj.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(n));
                }

                if (n < manifest.Items.Length)
                {
                    obj.GetComponent<CosmeticSelectButtonController>().CurrentCosmetic = manifest.Items[n];
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
            if (buttonIndex >= CurrentManifest.Items.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            Debug.Log($"Selected cosmetic {CurrentManifest.Items[buttonIndex]}");
        }
    }
}
