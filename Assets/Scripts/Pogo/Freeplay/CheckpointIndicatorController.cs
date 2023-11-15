using Pogo.Difficulties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Freeplay
{
    public class CheckpointIndicatorController : MonoBehaviour
    {
        PogoGameManager gameManager;
        Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
            gameManager = PogoGameManager.PogoInstance;
            gameManager.OnCustomCheckpointChanged += onCustomCheckpointChanged;
        }

        private void onCustomCheckpointChanged(object sender, EventArgs e)
        {
            ShowIndicator = gameManager.CustomRespawnActive && gameManager.CurrentDifficulty == Difficulty.Assist;
        }

        private bool showIndicator;

        public bool ShowIndicator
        {
            get => showIndicator; set
            {
                if (value == showIndicator) return;

                showIndicator = value;
                animator.SetBool("shouldShow", value);
            }
        }
    }
}
