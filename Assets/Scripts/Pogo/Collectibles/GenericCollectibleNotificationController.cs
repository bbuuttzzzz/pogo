using Pogo.Collectibles;
using TMPro;
using UnityEngine;

public class GenericCollectibleNotificationController : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI BodyText;
    public Transform IconParent;

    public void Initialize(CollectibleUnlockedEventArgs args)
    {
        if (!args.UnlockedGlobally && !string.IsNullOrEmpty(args.Collectible.GenericNotificationTitle_HalfUnlocked))
        {
            SetText(
                args.Collectible.GenericNotificationTitle_HalfUnlocked,
                args.Collectible.GenericNotificationBody_HalfUnlocked);
        }
        else
        {
            SetText(
                args.Collectible.GenericNotificationTitle,
                args.Collectible.GenericNotificationBody);
        }

        Instantiate(args.Collectible.GenericNotification3DIcon, IconParent);
    }

    private void SetText(string title, string body)
    {
        TitleText.text = title;
        BodyText.text = body;
    }
}