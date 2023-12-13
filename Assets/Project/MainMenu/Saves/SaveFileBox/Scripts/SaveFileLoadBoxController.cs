using Inputter;
using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Pogo.Saving
{
    public class SaveFileLoadBoxController : SaveFileGenericBoxController
    {
        [HideInInspector]
        public UnityEvent<SaveSlotIds> OnLoadTriggered;

        public Button MainButton;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DeathsText;
        public TextMeshProUGUI TimeText;
        public TextMeshProUGUI DifficultyNameText;
        public TextMeshProUGUI PercentText;
        public MeshFilter SkullMesh;
        public Renderer SkullMeshRenderer;
        public Transform ProgressBoxesParent;

        [SerializeField]
        private SaveSlotPreviewData previewData;

        public void Load()
        {
            OnLoadTriggered.Invoke(SlotId);
        }

        public void SetData(SaveSlotIds slotId, SaveSlotPreviewData data)
        {
            SlotId = slotId;
            previewData = data;
            UpdateDisplay();
        }

        [ContextMenu("Update Display")]
        private void UpdateDisplay()
        {
            DifficultyDescriptor difficulty;
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                difficulty = PogoGameManager.PogoInstance.DifficultyManifest.FindByKey(previewData.difficulty);
            }
            else
            {
                var difficulties = AssetDatabase.FindAssets($"t:{nameof(DifficultyDescriptor)}")
                        .Select(id => AssetDatabase.LoadAssetAtPath<DifficultyDescriptor>(AssetDatabase.GUIDToAssetPath(id)));

                difficulty = difficulties.Where(d => d.DifficultyEnum == previewData.difficulty)
                    .First();
            }
#else
        difficulty = PogoGameManager.PogoInstance.DifficultyManifest.FindByKey(previewData.difficulty);
#endif

            PercentText.text = FormatPercent(previewData.CompletionPerMille);
            TimeText.text = FormatTime(previewData.TotalMilliseconds);
            TitleText.text = previewData.name;
            DeathsText.text = $"x<b>{previewData.TotalDeaths}</b>";
            SetProgressBoxes();
            DifficultyNameText.text = difficulty.DisplayName;
            SkullMesh.sharedMesh = difficulty.SkullMesh;
            SkullMeshRenderer.sharedMaterial = difficulty.SkullMaterial;
        }

        private string FormatPercent(int completionPerMille)
        {
            if (completionPerMille % 10 == 0)
            {
                return $"{completionPerMille / 10}%";
            }
            decimal percent = completionPerMille * 0.1m;
            return $"{percent}%";
        }


        private string FormatTime(long totalMilliseconds)
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

        private void SetProgressBoxes()
        {
            for (int n = 0; n < ProgressBoxesParent.childCount; n++)
            {
                SaveFileProgressBoxController progressBox = ProgressBoxesParent.GetChild(n).GetComponent<SaveFileProgressBoxController>();

                if (n <= previewData.LastFinishedChapter)
                {
                    progressBox.SetState(SaveFileProgressBoxController.States.Finished);
                    progressBox.SetProgress(previewData.ChapterProgresses[n]);
                }
                else
                {
                    progressBox.SetState(SaveFileProgressBoxController.States.Finished);
                    progressBox.SetProgress(0);
                }
            }
        }
    }
}