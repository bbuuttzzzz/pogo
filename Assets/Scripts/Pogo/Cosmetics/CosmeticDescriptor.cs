using Pogo.Collectibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Cosmetics
{
    public abstract class CosmeticDescriptor : ScriptableObject
    {
        public enum UnlockTypes
        {
            AlwaysUnlocked,
            VendingMachine,
            Collectible
        }

        public UnlockTypes UnlockType;
        public string DisplayName;
        public Sprite Icon;

        public bool AllowRecoloring;

        [HideInInspector]
        public CollectibleDescriptor Collectible;
    }
}
