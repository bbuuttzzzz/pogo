using Pogo.Cosmetics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VendingMachineManifest", menuName = "Pogo/Cosmetics/VendingMachineManifest")]

public class VendingMachineManifest : ScriptableObject
{
    public VendingMachineEntry[] Entries;
}
