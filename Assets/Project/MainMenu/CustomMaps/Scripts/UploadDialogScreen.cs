using Pogo;
using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.Steam;
using Pogo.MainMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace Pogo.CustomMaps.UI
{
    [AddComponentMenu("Pogo.CustomMaps.UI.UploadDialogScreen")]
    public class UploadDialogScreen : MonoBehaviour
    {
        private PogoGameManager gameManager;
        public CustomMapsRoot parent;
        public Transform ChecklistEntriesParent;
        public GameObject ChecklistEntryPrefab;

        public Button UploadButton;
        private bool ChecklistComplete;

        private Dictionary<ChecklistEntryIds, ChecklistEntry> ChecklistEntries;

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;
            UploadButton.onClick.AddListener(UploadCurrentMap);
            GenerateChecklistItems();
        }

        private void OnEnable()
        {
            if (parent != null && parent.CurrentMap != null)
            {
                UpdateChecklist();
            }
        }

        private void UpdateChecklist()
        {
            ChecklistEntries[ChecklistEntryIds.BspFile].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = parent.CurrentMap.BspPath != null,
                Value = parent.CurrentMap.BspPath != null ? $"{parent.CurrentMap.MapName}.bsp" : null
            });
            ChecklistEntries[ChecklistEntryIds.PreviewSprite].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = parent.CurrentMap.PreviewImagePath != null,
                Value = parent.CurrentMap.PreviewImagePath != null ? "thumbnail.png" : null
            });
            ChecklistEntries[ChecklistEntryIds.Author].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = !string.IsNullOrEmpty(parent.CurrentMap.AuthorName),
                Value = parent.CurrentMap.AuthorName
            });
            ChecklistEntries[ChecklistEntryIds.Version].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = !string.IsNullOrEmpty(parent.CurrentMap.Version),
                Value = parent.CurrentMap.Version
            });
            ChecklistEntries[ChecklistEntryIds.WorshopId].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = parent.CurrentMap.WorkshopId != null,
                Value = parent.CurrentMap.WorkshopId?.ToString()
            });

            ChecklistComplete = ChecklistEntries.Values.Any(entry => entry.Data.IsRequired && !entry.Data.Status.IsComplete);
            UploadButton.interactable = ChecklistComplete;
        }

        private void UploadCurrentMap() => UploadMap(parent.CurrentMap);

        private void UploadMap(MapHeader header)
        {
#if !DISABLESTEAMWORKS
            if (header.WorkshopId == null)
            {
                gameManager.WorkshopUploadService.CreateAndUpdateMap(header, UploadMap_Callback);
            }
            else
            {
                gameManager.WorkshopUploadService.UpdateMap(header, UploadMap_Callback);
            }
#endif
        }

        private void UploadMap_Callback(UpdateMapResult result)
        {
            if (!result.Success)
            {
                Debug.LogError($"STEAMWORKS issue Creating a new map: {result.ErrorMessage}");
                parent.parent.OpenPopup(new MainMenu.MenuPopupData()
                {
                    Title = "There was an error uploading your map!",
                    Body = result.ErrorMessage,
                    OkText = "Ok",
                });
                return;
            }

#if !DISABLESTEAMWORKS
            parent.parent.OpenPopup(new MainMenu.MenuPopupData()
            {
                Title = "Success!",
                Body = "Would you like to view the workshop page?",
                OkText = "Open Link",
                OkPressedCallback = () =>
                {
                    gameManager.WorkshopUploadService.OpenMapWebpage(result.UpdatedHeader);
                },
                CancelText = "Close"
            });
#endif
        }

        #region Checklist

        public enum ChecklistEntryIds
        {
            BspFile,
            PreviewSprite,
            Author,
            Version,
            WorshopId
        }

        private void GenerateChecklistItems()
        {
            ChecklistEntries = new Dictionary<ChecklistEntryIds, ChecklistEntry>();
            AddChecklistItem(ChecklistEntryIds.BspFile, new ChecklistEntryData()
            {
                Title = "\'.bsp\' File",
                IsRequired = true,
                HintBody = "Contains the map's geometry and data.\n Compiled from the \'.map\' file using an external tool.",
                DefaultDisplayValue = "Missing"
            });
            AddChecklistItem(ChecklistEntryIds.PreviewSprite, new ChecklistEntryData()
            {
                Title = MapHeaderHelper.previewSpriteFileName,
                IsRequired = false,
                HintBody = "A 4x3 \'.png\' preview image for the map.\nIf your map has an info_camera_preview entity, you can generate it using the refresh button",
                AutoCompleteAction = PromptGeneratePreviewImage,
                AutoCompleteMode = ChecklistEntryData.AutoCompleteModes.Show,
                DefaultDisplayValue = "No Thumbnail"
            });
            // CFG file stuff
            AddChecklistItem(ChecklistEntryIds.Author, new ChecklistEntryData()
            {
                Title = "Author Name",
                IsRequired = false,
                HintBody = $"Define this in {MapHeaderHelper.mapDefinitionFileName} as \'Author: <name>\'",
                DefaultDisplayValue = "Default (Anonymous)"
            });
            AddChecklistItem(ChecklistEntryIds.Version, new ChecklistEntryData()
            {
                Title = "Version",
                IsRequired = false,
                HintBody = $"Define this in {MapHeaderHelper.mapDefinitionFileName} as \'Version: <x.y.z>\'",
                DefaultDisplayValue = "Default (0.1.0)"
            });
            AddChecklistItem(ChecklistEntryIds.WorshopId, new ChecklistEntryData()
            {
                Title = "Workshop ID",
                IsRequired = false,
                AutoCompleteAction = PromptToResetWorkshopId,
                AutoCompleteMode = ChecklistEntryData.AutoCompleteModes.ShowWhenComplete,
                HintBody = "Uniquely identifies this map on the steam workshop.",
                DefaultDisplayValue = "Generate On Upload"
            });

        }

        private void AddChecklistItem(ChecklistEntryIds id, ChecklistEntryData item)
        {
            ChecklistEntry newEntry = Instantiate(ChecklistEntryPrefab, ChecklistEntriesParent).GetComponent<ChecklistEntry>();
            newEntry.gameObject.name = $"ChecklistEntry {item.Title}";
            newEntry.Set(item);
            newEntry.OnShowHint.AddListener(ShowHint);

            ChecklistEntries.Add(id, newEntry);
        }

        private void ShowHint(ChecklistEntryData entryData)
        {
            parent.parent.OpenPopup(new MainMenu.MenuPopupData()
            { 
                Title = entryData.Title,
                Body = entryData.HintBody,
                OkText = "Close"
            });
        }

        #endregion

        private void PromptGeneratePreviewImage()
        {
            parent.parent.OpenPopup(new MainMenu.MenuPopupData()
            {
                Title = "Really Regenerate Preview Image?",
                Body = "This will load the map",
                OkText = "Yes",
                OkPressedCallback = GeneratePreviewImage,
                CancelText = "Nevermind"
            });
        }

        private void GeneratePreviewImage()
        {
            gameManager.CustomMapBuilder.LoadMapAndGenerateThumbnail(parent.CurrentMap, OnAfterGeneratePreviewImage);
        }

        private void OnAfterGeneratePreviewImage(GenerateMapThumbnailResult result)
        {
            Debug.Log($"Returning to main menu after generating map thumbnail... (result: {result.ResultType})");
            gameManager.LoadControlSceneAsync(gameManager.MainMenuControlScene, () =>
            {
                ReturnToUploadDialogAndShowThumbnailResult(gameManager, result);
            });
        }

        private static void ReturnToUploadDialogAndShowThumbnailResult(PogoGameManager gameManager, GenerateMapThumbnailResult result)
        {
            Debug.Log("Waiting for MainMenu to load to return to Upload Map Dialog...");
            gameManager.DoMainMenuAction((mainMenu) =>
            {
                Debug.Log("Returning to upload map dialog after generating map thumbnail...");
                mainMenu.OpenCustomMapsScreenInstantly();
                mainMenu.MapsRoot.CurrentMap = result.MapHeader;
                //mainMenu.MapsRoot.UploadScreen.UpdateChecklist();
                mainMenu.MapsRoot.OverrideOpenMapScreen = CustomMapsRoot.ScreenIds.UploadDialog;

                MenuPopupData popupData = new MainMenu.MenuPopupData()
                {
                    OkText = "Close",
                };

                popupData.Title = result.ResultType switch
                {
                    GenerateMapThumbnailResult.ResultTypes.Success => "Success!",
                    _ => "Failure!"
                };

                popupData.Body = result.ResultType switch
                {
                    GenerateMapThumbnailResult.ResultTypes.Success => "Thumbnail image successfully updated",
                    GenerateMapThumbnailResult.ResultTypes.FailureMissingEntity => "No info_camera_preview was found.",
                    _ => "Failed for an unknown reason. See more in Player.Log"
                };

                mainMenu.OpenPopup(popupData);
            });
        }

        private void PromptToResetWorkshopId()
        {
            parent.parent.OpenPopup(new MainMenu.MenuPopupData()
            {
                Title = "Really Reset Workshop ID?",
                Body = "This will upload the map to the steam workshop as a new listing",
                OkText = "Yes",
                OkPressedCallback = ResetWorkshopId,
                CancelText = "Nevermind"
            });
        }

        private void ResetWorkshopId()
        {
            parent.CurrentMap.WorkshopId = null;
            MapHeaderHelper.SaveMapHeaderConfig(parent.CurrentMap);
            UpdateChecklist();
        }
    }
}
