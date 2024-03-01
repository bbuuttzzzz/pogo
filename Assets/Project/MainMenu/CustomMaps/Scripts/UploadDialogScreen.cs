using Pogo;
using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.Steam;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.CustomMaps.UI
{
    [AddComponentMenu("Pogo.CustomMaps.UI.UploadDialogScreen")]
    public class UploadDialogScreen : MonoBehaviour
    {
        private PogoGameManager gameManager;
        public CustomMapsRoot parent;
        public Transform ChecklistEntriesParent;
        public GameObject ChecklistEntryPrefab;

        private List<ChecklistEntry> ChecklistEntries;

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;
            
        }

        private void OnEnable()
        {
            UpdateMap();
        }

        private void UpdateMap()
        {
            RegenerateChecklistItems();
        }

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
                    ShowOkButton = true,
                    OkText = "Ok",
                });
                return;
            }

#if !DISABLESTEAMWORKS
            parent.parent.OpenPopup(new MainMenu.MenuPopupData()
            {
                Title = "Success!",
                Body = "Would you like to view the workshop page?",
                ShowOkButton = true,
                OkText = "Open Link",
                OkPressedCallback = () =>
                {
                    gameManager.WorkshopUploadService.OpenMapWebpage(result.UpdatedHeader);
                },
                ShowCancelButton = true,
                CancelText = "Close"
            });
#endif
        }

        #region Checklist

        private void RegenerateChecklistItems()
        {
            if (ChecklistEntries != null)
            {
                foreach(var item in ChecklistEntries)
                {
                    Destroy(item.gameObject);
                }
            }

            ChecklistEntries = new List<ChecklistEntry>();
            AddChecklistItem(new ChecklistEntryData()
            {
                Title = ".bsp file",
                IsRequired = true,
                IsComplete = parent.CurrentMap.BspPath != null,
                HintBody = ".bsp files contain the map's geometry and data. .map files must be compiled into .bsp"
            });
            AddChecklistItem(new ChecklistEntryData()
            {
                Title = IndexingHelper.previewSpriteFileName,
                IsRequired = false,
                IsComplete = parent.CurrentMap.PreviewImagePath != null,
                HintBody = "a 4x3 .png preview image for the map. use an info_camera_preview to generate an ingame thumbnail",
                AllowAutoCompleteWhenCompleted = true,
                AutoCompleteAction = GeneratePreviewImage
            });
            // CFG file stuff
            AddChecklistItem(new ChecklistEntryData()
            {
                Title = "Author Name",
                IsRequired = false,
                IsComplete = !string.IsNullOrEmpty(parent.CurrentMap.AuthorName),
                HintBody = $"Specify this in the {IndexingHelper.mapDefinitionFileName} as \'Author: <name>\'"
            });
            AddChecklistItem(new ChecklistEntryData()
            {
                Title = "Version",
                IsRequired = false,
                IsComplete = !string.IsNullOrEmpty(parent.CurrentMap.Version),
                HintBody = $"Specify this in the {IndexingHelper.mapDefinitionFileName} as \'Version: <X.Y.Z>\' where x y z are major, minor, and revision numbers"
            });
            AddChecklistItem(new ChecklistEntryData()
            {
                Title = "Workshop ID",
                IsRequired = true,
                IsComplete = parent.CurrentMap.WorkshopId != null,
                AutoCompleteAction = GenerateWorkshopId
            });

        }

        private void AddChecklistItem(ChecklistEntryData item)
        {
            var newEntry = Instantiate(ChecklistEntryPrefab, ChecklistEntriesParent).GetComponent<ChecklistEntry>();
            newEntry.Set(item, ShowHint);

            ChecklistEntries.Add(newEntry);
        }

        private void ShowHint(string obj)
        {
            throw new NotImplementedException();
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
