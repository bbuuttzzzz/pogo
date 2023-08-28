using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static Pogo.PogoGameManager;

namespace Pogo.Saving
{
    public class DifficultyButtonController : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent<DifficultyDescriptor> OnDifficultySelected;

        public DifficultyDescriptor Difficulty;

        public TextMeshProUGUI DifficultyNameText;
        public MeshFilter SkullMesh;
        public Renderer SkullMeshRenderer;
        public TextMeshProUGUI DescriptionText;

        public void SetDifficulty(DifficultyDescriptor difficulty)
        {
            Difficulty = difficulty;
            UpdateDisplay();
        }

        public void Select()
        {
            OnDifficultySelected?.Invoke(Difficulty);
        }

        [ContextMenu("Update Display")]
        private void UpdateDisplay()
        {
            DifficultyNameText.text = Difficulty.DisplayName;
            SkullMesh.sharedMesh = Difficulty.SkullMesh;
            SkullMeshRenderer.sharedMaterial = Difficulty.SkullMaterial;
            DescriptionText.text = Difficulty.Description;
        }
    }
}
