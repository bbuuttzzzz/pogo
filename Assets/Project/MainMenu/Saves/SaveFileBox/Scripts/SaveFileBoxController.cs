using Pogo;
using Pogo.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SaveFileBoxController : MonoBehaviour
{
    public int SlotNumber;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DeathsText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI DifficultyNameText;
    public MeshFilter SkullMesh;
    public Renderer SkullMeshRenderer;
    public Transform ProgressBoxesParent;

    [SerializeField]
    private SaveSlotPreviewData previewData;

    public void SetPreviewData(SaveSlotPreviewData data)
    {

    }

    private void UpdateDisplay()
    {
        DifficultyDescriptor difficulty;
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            difficulty = PogoGameManager.PogoInstance.DifficultyManifest.GetDifficulty(previewData.difficulty);
        }
        else
        {
            var difficulties = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(DifficultyDescriptor)}")
                    .Select(id => AssetDatabase.LoadAssetAtPath<DifficultyDescriptor>(AssetDatabase.GUIDToAssetPath(id)));

            difficulty = difficulties.Where(d => d.DifficultyEnum == previewData.difficulty)
                .First();
        }
#else
        difficulty = PogoGameManager.PogoInstance.DifficultyManifest.GetDifficulty(previewData.difficulty);
#endif

        TimeText.text = FormatTime(previewData.TotalMilliseconds);
        TitleText.text = previewData.name;
        DeathsText.text = $"x<b>{previewData.TotalDeaths}</b>";
        DifficultyNameText.text = difficulty.DisplayName;
        SkullMesh.sharedMesh = difficulty.SkullMesh;
        SkullMeshRenderer.sharedMaterial = difficulty.SkullMaterial;
    }

    private string FormatTime(int totalMilliseconds)
    {
        var timespan = TimeSpan.FromMilliseconds(totalMilliseconds);

        if (timespan.TotalHours >= 1)
        {
            return $"{(int)timespan.TotalHours}:{timespan:mm\\:ss\\.fff}";
        }
        else if (timespan.TotalMinutes >= 1)
        {
            return $"{(int)timespan.TotalMinutes}:{timespan:ss\\.fff}";
        }
        else
        {
            return $"0:{timespan:ss\\.fff}";
        }
    }

    #region Editor

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UpdateDisplay();
        }
    }

    #endregion
}
