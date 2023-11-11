using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Cosmetics
{
    public class VendingMachineButtonController : MonoBehaviour
    {
        public Outline outline;
        public float OutlineWidth
        {
            get => outline.OutlineWidth;
            set => outline.OutlineWidth = value;
        }
        
        public Color OutlineColor
        {
            get => outline.OutlineColor;
            set => outline.OutlineColor = value;
        }
    }
}
