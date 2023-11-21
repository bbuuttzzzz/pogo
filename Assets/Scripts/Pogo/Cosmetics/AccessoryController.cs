using Players.Visuals;
using System;
using UnityEngine;

namespace Pogo.Cosmetics
{
    public class AccessoryController : MonoBehaviour
    {
        public PlayerModelAttachment Attachment;

        public void Start()
        {
            var player = GetComponentInParent<IPlayerModelControllerProvider>(true);
            if (player == null)
            {
                Debug.LogWarning("missing PlayerController above AccessoryController", this);
                return;
            }
            
            if (Attachment != null)
            {
                player.OnModelControllerChanged?.AddListener(Player_OnModelControllerChanged);
                if (player.PlayerModelController != null)
                {
                    player.PlayerModelController.AddAttachment(Attachment);
                }
            }
        }

        private void Player_OnModelControllerChanged(PlayerModelController arg0)
        {
            // ...LETS JUST PRAY THE OLD PLAYERMODEL CLEANED THIS UP FOR U :   ^   )
            arg0.AddAttachment(Attachment);
        }
    }
}
