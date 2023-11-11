using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.Cosmetics
{
    public class CosmeticSlotMenuController : MonoBehaviour
    {
        public CosmeticManifest manifest;

        public void OnEnable()
        {
            UpdateButtons();
        }
        
        public void UpdateButtons()
        {
            for (int n = 0; n < transform.childCount; n++)
            {
                var button = transform.GetChild(n);
                var controller = button.GetComponent<CosmeticSelectButtonController>();

                var newCosmetic = PogoGameManager.PogoInstance.CurrentGlobalDataTracker.GetCosmetic(controller.CurrentCosmetic.Slot, controller.CurrentCosmetic.Key);
                if (controller.CurrentCosmetic.Key != newCosmetic.Key)
                {
                    controller.CurrentCosmetic = manifest.Find(newCosmetic.Slot).FindByKey(newCosmetic.Key);
                }
            }
        }
    }
}
