using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.Saving
{
    public static class SaveSlotConstants
    {
        public const int SaveSlotCount = 3;

        public static int SaveSlotIdToIndex(SaveSlotIds id)
        {
            return (int)id;
        }

        public static SaveSlotIds SaveSlotIdFromIndex(int index)
        {
            return (SaveSlotIds)index;
        }

        public static string SaveSlotPath(SaveSlotIds id)
        {
            return (SaveSlotIdToIndex(id) + 1).ToString();
        }

        public static string SaveSlotName(SaveSlotIds selectedSlot)
        {
            return $"Slot {SaveSlotPath(selectedSlot)}";
        }
    }

    public enum SaveSlotIds
    {
        Slot1 = 0,
        Slot2 = 1,
        Slot3 = 2,
    }
}
