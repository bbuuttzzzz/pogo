using Pogo;
using Pogo.Cosmetics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericCosmeticNotificationController : MonoBehaviour
{
    public Text TitleText;
    public Text BodyText;
    public Image IconImage;

    public void Initialize(CosmeticDescriptor cosmetic)
    {
        TitleText.text = GenerateTitleText(cosmetic);
        BodyText.text = GenerateBodyText(cosmetic);
        IconImage.sprite = cosmetic.Icon;
    }

    private static string GenerateTitleText(CosmeticDescriptor cosmetic)
    {
        var slot = PogoGameManager.PogoInstance.CosmeticManifest.Find(cosmetic.Slot);
        return $"New {slot.DisplayName} Unlocked! - {cosmetic.DisplayName}";
    }
    private string GenerateBodyText(CosmeticDescriptor cosmetic)
    {
        if (!string.IsNullOrWhiteSpace(cosmetic.OverrideUnlockText))
        {
            return cosmetic.OverrideUnlockText;
        }

        switch (cosmetic.UnlockType)
        {
            case CosmeticDescriptor.UnlockTypes.AlwaysUnlocked:
                return "...Not sure how that happened XD";
            case CosmeticDescriptor.UnlockTypes.VendingMachine:
                if (PogoGameManager.PogoInstance.CosmeticManifest.Vending.TryFind(cosmetic, out var result))
                {
                    return $"You Unlocked this by finding {result.Cost} Quarters!";
                }
                return "";
            case CosmeticDescriptor.UnlockTypes.Collectible:
                return "Equip it from the 'Appearance' Screen!";
        }
        return "";
    }

}
