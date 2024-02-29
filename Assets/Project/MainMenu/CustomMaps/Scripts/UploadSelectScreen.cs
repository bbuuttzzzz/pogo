using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WizardUI;
using WizardUtils;

namespace Pogo.CustomMaps.UI
{
    [AddComponentMenu("Pogo.CustomMaps.UI.UploadSelectScreen")]
    public class UploadSelectScreen : MonoBehaviour
    {
        private PogoGameManager gameManager;
        public CustomMapsRoot parent;

        public int LoadMoreCount = 5;
        private bool CanLoadMore;
        public GameObject ButtonPrefab;
        public Transform ButtonsRoot;
        public ScrollerLoadMore Scroller;
        private List<CustomMapButton> Buttons;
        private IEnumerator<MapHeader> UnloadedHeaders;

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;

            Scroller.OnShouldLoadMore.AddListener(() => LoadMore(LoadMoreCount));
        }


        private void OnEnable()
        {
            ResetButtons();
        }

        private void ResetButtons()
        {
            if (Buttons != null)
            {
                foreach (var button in Buttons)
                {
                    Destroy(button.gameObject);
                }
            }
            Buttons = new List<CustomMapButton>();
            UnloadedHeaders = gameManager.CustomMapBuilder
                .GetMapHeaders(localOnly: true)
                .GetEnumerator();
            CanLoadMore = true;
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
                return;
            }

            OpenUploadMapDialog(result.UpdatedHeader);
        }

        private void OpenUploadMapDialog(MapHeader header)
        {

        }

        private void AddButton(MapHeader header)
        {
            var obj = Instantiate(ButtonPrefab, ButtonsRoot);
            var button = obj.GetComponent<CustomMapButton>();
            button.Header = header;
            button.UIButton.onClick.AddListener(() =>
            {
                OpenUploadMapDialog(header);
            });
            Buttons.Add(button);
        }

        public void LoadMore(int count = -1)
        {
            if (count < 0) count = LoadMoreCount;

            if (!CanLoadMore)
            {
                return;
            }

            for (int n = 0; n < count; n++)
            {
                if (!UnloadedHeaders.MoveNext())
                {
                    CanLoadMore = false;
                    break;
                }
                AddButton(UnloadedHeaders.Current);
            }
        }
    }
}
