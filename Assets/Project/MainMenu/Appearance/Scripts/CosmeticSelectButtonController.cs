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
        public Button Button {get; private set;}

        private void Awake()
        {
            Button = GetComponent<Button>();
        }

        public CosmeticDescriptor CurrentCosmetic
        {
            get => currentCosmetic;
            set
            {
                CurrentCosmetic = value;
                UpdateDisplay();
            }
        }

        [SerializeField]
        private CosmeticDescriptor currentCosmetic;
        public Image IconImage;

        [ContextMenu("Update Display")]
        private void UpdateDisplay()
        {
            IconImage.sprite = CurrentCosmetic.Icon;
        }
    }
}
