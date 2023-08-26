using Pogo;
using Pogo.Saving;
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
    public TextMeshProUGUI DifficultyNameText;
    public MeshFilter SkullMesh;
    public Renderer SkullMeshRenderer;

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

#endif

        TitleText.text = previewData.name;
        DeathsText.text = $"x<b>{previewData.TotalDeaths}</b>";
        DifficultyNameText.text = difficulty.DisplayName;
        SkullMesh.sharedMesh = difficulty.SkullMesh;
        SkullMeshRenderer.sharedMaterial = difficulty.SkullMaterial;
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
