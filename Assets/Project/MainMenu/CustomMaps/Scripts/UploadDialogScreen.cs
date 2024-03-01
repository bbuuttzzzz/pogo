using Pogo;
using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
            UpdateMap();
        }

        private void UpdateMap()
        {
            ChecklistEntries[ChecklistEntryIds.BspFile].SetComplete(parent.CurrentMap.BspPath != null);
            ChecklistEntries[ChecklistEntryIds.PreviewSprite].SetComplete(parent.CurrentMap.PreviewImagePath != null);
            ChecklistEntries[ChecklistEntryIds.Author].SetComplete(!string.IsNullOrEmpty(parent.CurrentMap.AuthorName));
            ChecklistEntries[ChecklistEntryIds.Version].SetComplete(!string.IsNullOrEmpty(parent.CurrentMap.Version));
            ChecklistEntries[ChecklistEntryIds.WorshopId].SetComplete(parent.CurrentMap.WorkshopId != null);

            ChecklistComplete = ChecklistEntries.Values.Any(entry => entry.Data.IsRequired && !entry.Data.IsComplete);
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
                Title = ".bsp file",
                IsRequired = true,
                HintBody = ".bsp files contain the map's geometry and data. .map files must be compiled into .bsp"
            });
            AddChecklistItem(ChecklistEntryIds.PreviewSprite, new ChecklistEntryData()
            {
                Title = IndexingHelper.previewSpriteFileName,
                IsRequired = false,
                HintBody = "a 4x3 .png preview image for the map. use an info_camera_preview to generate an ingame thumbnail",
                AllowAutoCompleteWhenCompleted = true,
                AutoCompleteAction = GeneratePreviewImage
            });
            // CFG file stuff
            AddChecklistItem(ChecklistEntryIds.Author, new ChecklistEntryData()
            {
                Title = "Author Name",
                IsRequired = false,
                HintBody = $"Specify this in the {IndexingHelper.mapDefinitionFileName} as \'Author: <name>\'"
            });
            AddChecklistItem(ChecklistEntryIds.Version, new ChecklistEntryData()
            {
                Title = "Version",
                IsRequired = false,
                HintBody = $"Specify this in the {IndexingHelper.mapDefinitionFileName} as \'Version: <X.Y.Z>\' where x y z are major, minor, and revision numbers"
            });
            AddChecklistItem(ChecklistEntryIds.WorshopId, new ChecklistEntryData()
            {
                Title = "Workshop ID",
                IsRequired = true,
                AutoCompleteAction = GenerateWorkshopId
            });

        }

        private void AddChecklistItem(ChecklistEntryIds id, ChecklistEntryData item)
        {
            ChecklistEntry newEntry = Instantiate(ChecklistEntryPrefab, ChecklistEntriesParent).GetComponent<ChecklistEntry>();
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
