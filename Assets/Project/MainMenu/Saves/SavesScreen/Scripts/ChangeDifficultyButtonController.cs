using Pogo;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static Pogo.PogoGameManager;

public class ChangeDifficultyButtonController : MonoBehaviour
{
    public UnityEvent OnPressed;

    public DifficultyDescriptor Difficulty;

    public TextMeshProUGUI DifficultyNameText;
    public MeshFilter SkullMesh;
    public Renderer SkullMeshRenderer;

    public void SetDifficulty(DifficultyDescriptor difficulty)
    {
        Difficulty = difficulty;
        UpdateDisplay();
    }

    [ContextMenu("Update Display")]
    private void UpdateDisplay()
    {
        DifficultyNameText.text = Difficulty.DisplayName;
        SkullMesh.sharedMesh = Difficulty.SkullMesh;
        SkullMeshRenderer.sharedMaterial = Difficulty.SkullMaterial;
    }

    public void Press()
    {
        OnPressed?.Invoke();
    }
}
