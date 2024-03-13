using Pogo.CustomMaps.Indexing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WizardUI;
using WizardUtils;
using ScrollView = UnityEngine.UIElements.ScrollView;

namespace Pogo.CustomMaps.UI
{
    public class CustomMapSelectScreen : MonoBehaviour
    {
        private PogoGameManager gameManager;

        public int LoadMoreCount = 5;
        private bool CanLoadMore;
        public GameObject ButtonPrefab;
        public Transform ButtonsRoot;
        public ScrollView Scroller;
        private List<CustomMapButton> Buttons;
        private IEnumerator<MapHeader> UnloadedHeaders;

        public Button UploadButton;
        public Button BrowseLocalButton;
        public Button ViewWorkshopButton;

        [HideInInspector]
        public UnityEvent OnUploadPressed;

        private void Awake()
        {
            gameManager = PogoGameManager.PogoInstance;

            if (!gameManager.PlatformService.SupportsWorkshop)
            {
                UploadButton.gameObject.SetActive(false);
                ViewWorkshopButton.gameObject.SetActive(false);
            }
            else
            {
                UploadButton.onClick.AddListener(() => OnUploadPressed?.Invoke());
                ViewWorkshopButton.onClick.AddListener(() => Application.OpenURL(gameManager.PlatformService.WorkshopLink));
            }

            BrowseLocalButton.onClick.AddListener(() => Application.OpenURL($"file:///{gameManager.CustomMapBuilder.WadFolderRootPath}"));
        }


        private void OnEnable()
        {
            ResetButtons();
            LoadMore(100);
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
            UnloadedHeaders = gameManager.CustomMapBuilder.GetMapHeaders().GetEnumerator();
            CanLoadMore = true;
        }


        private void SelectMap(MapHeader header)
        {
            gameManager.CustomMapBuilder.LoadCustomMapLevel(header);
        }

        private void AddButton(MapHeader header)
        {
            var obj = Instantiate(ButtonPrefab, ButtonsRoot);
            var button = obj.GetComponent<CustomMapButton>();
            button.Header = header;
            button.UIButton.onClick.AddListener(() =>
            {
                SelectMap(header);
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
