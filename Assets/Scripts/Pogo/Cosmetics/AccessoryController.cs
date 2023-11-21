using Assets.Scripts.Player;
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
            var player = GetComponentInParent<PlayerController>(true);

            if (player == null)
            {
                return;
            }
            player.OnModelControllerChanged?.AddListener(Player_OnModelControllerChanged);
            if (player.CurrentModelController != null)
            {
                player.CurrentModelController.AddAttachment(Attachment);
            }
        }

        private void Player_OnModelControllerChanged(PlayerModelController arg0)
        {
            // ...LETS JUST PRAY THE OLD PLAYERMODEL CLEANED THIS UP FOR U :   ^   )
            arg0.AddAttachment(Attachment);
        }
    }
}
