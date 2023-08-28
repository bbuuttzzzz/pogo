using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo.Saving
{
    public class SaveFileNewBoxController : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent<SaveSlotIds> OnNewGameTriggered;

        public SaveSlotIds SlotId;

        public void NewGame()
        {
            OnNewGameTriggered?.Invoke(SlotId);
        }
    }
}
