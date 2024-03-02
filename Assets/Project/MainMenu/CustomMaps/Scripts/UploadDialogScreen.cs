using Pogo;
using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.Steam;
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
                UpdateMap();
            }
        }

        private void UpdateMap()
        {
            ChecklistEntries[ChecklistEntryIds.BspFile].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = parent.CurrentMap.BspPath != null,
                Value = parent.CurrentMap.BspPath != null ? $"{parent.CurrentMap.MapName}.bsp" : "missing"
            });
            ChecklistEntries[ChecklistEntryIds.PreviewSprite].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = parent.CurrentMap.PreviewImagePath != null,
                Value = parent.CurrentMap.PreviewImagePath != null ? "thumbnail.png" : "no thumbnail"
            });
            ChecklistEntries[ChecklistEntryIds.Author].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = !string.IsNullOrEmpty(parent.CurrentMap.AuthorName),
                Value = parent.CurrentMap.AuthorName ?? "default (anonymous)"
            });
            ChecklistEntries[ChecklistEntryIds.Version].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = !string.IsNullOrEmpty(parent.CurrentMap.Version),
                Value = parent.CurrentMap.Version ?? "default (0.1.0)"
            });
            ChecklistEntries[ChecklistEntryIds.WorshopId].SetStatus(new ChecklistEntryStatus()
            {
                IsComplete = parent.CurrentMap.WorkshopId != null,
                Value = parent.CurrentMap.WorkshopId?.ToString() ?? "missing"
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
                HintBody = "Contains the map's geometry and data.\n Compiled from the \'.map\' file."
            });
            AddChecklistItem(ChecklistEntryIds.PreviewSprite, new ChecklistEntryData()
            {
                Title = MapHeaderHelper.previewSpriteFileName,
                IsRequired = false,
                HintBody = "A 4x3 \'.png\' preview image for the map.\nIf your map has an info_camera_preview entity, you can generate it using the refresh button",
                AllowAutoCompleteWhenCompleted = true,
                AutoCompleteAction = GeneratePreviewImage
            });
            // CFG file stuff
            AddChecklistItem(ChecklistEntryIds.Author, new ChecklistEntryData()
            {
                Title = "Author Name",
                IsRequired = false,
                HintBody = $"Define this in the {MapHeaderHelper.mapDefinitionFileName} as \'Author: <name>\'"
            });
            AddChecklistItem(ChecklistEntryIds.Version, new ChecklistEntryData()
            {
                Title = "Version",
                IsRequired = false,
                HintBody = $"Define this in {MapHeaderHelper.mapDefinitionFileName} as \'Version: <x.y.z>\'"
            });
            AddChecklistItem(ChecklistEntryIds.WorshopId, new ChecklistEntryData()
            {
                Title = "Workshop ID",
                IsRequired = true,
                AutoCompleteAction = GenerateWorkshopId,
                HintBody = "Uniquely identifies this map on the steam workshop.\nGenerate it using the refresh button."
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

        private void GeneratePreviewImage()
        {
            throw new NotImplementedException();
        }


        private void GenerateWorkshopId()
        {
            throw new NotImplementedException();
        }
    }
}
