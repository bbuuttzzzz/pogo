using Pogo.Cosmetics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "VendingMachineManifest", menuName = "Pogo/Cosmetics/VendingMachineManifest")]

public class VendingMachineManifest : ScriptableObject
{
    public VendingMachineEntry[] Entries;

    public void Sort()
    {
        Entries = Entries.OrderBy(x => x.UnlockThreshold).ToArray();
    }

    public bool TryFind(CosmeticDescriptor cosmetic, out VendingMachineEntry result)
    {
        foreach(VendingMachineEntry entry in Entries)
        {
            if (entry.Cosmetic == cosmetic)
            {
                result = entry;
                return true;
            }
        }

        result = default;
        return false;
    }

    public bool TryGetNext(int lastUnlockedCost, out VendingMachineEntry result)
    {
        int index = GetNextIndex(lastUnlockedCost);
        if (index > -1)
        {
            result = Entries[index];
            return true;
        }

        result = default;
        return false;
    }

    public int GetNextIndex(int lastUnlockedCost)
    {
        if (Entries[^1].UnlockThreshold < lastUnlockedCost)
        {
            return -1;
        }

        int low = 0;
        int high = Entries.Length - 1;

        while (low < high)
        {
            int median = low + (high - low) / 2;
            if (Entries[median].UnlockThreshold > lastUnlockedCost)
            {
                high = median;
            }
            else
            {
                low = median + 1;
            }
        }

        return low;
    }
}
