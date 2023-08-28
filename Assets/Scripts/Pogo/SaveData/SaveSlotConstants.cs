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
            return id switch
            {
                SaveSlotIds.Slot1 => 0,
                SaveSlotIds.Slot2 => 1,
                SaveSlotIds.Slot3 => 2,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static SaveSlotIds SaveSlotIdFromIndex(int index)
        {
            return index switch
            {
                0 => SaveSlotIds.Slot1,
                1 => SaveSlotIds.Slot2,
                2 => SaveSlotIds.Slot3,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static string SaveSlotPath(SaveSlotIds id)
        {
            return id switch
            {
                SaveSlotIds.Slot1 => "1",
                SaveSlotIds.Slot2 => "2",
                SaveSlotIds.Slot3 => "3",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public enum SaveSlotIds
    {
        Slot1,
        Slot2,
        Slot3,
    }
}
