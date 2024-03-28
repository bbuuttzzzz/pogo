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
        public GameObject ErrorMessagePrefab;
        public Transform MapsRoot;
        public ScrollView Scroller;
        private List<GameObject> MapsGameObjects;
        private IEnumerator<GenerateMapHeaderResult> UnloadedHeaders;

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

            BrowseLocalButton.onClick.AddListener(() => Application.OpenURL($"file:///{gameManager.CustomMapBuilder.MapsFolderRootPath}"));
        }


        private void OnEnable()
        {
            ResetButtons();
            LoadMore(100);
        }

        private void ResetButtons()
        {
            if (MapsGameObjects != null)
            {
                foreach (var button in MapsGameObjects)
                {
                    Destroy(button.gameObject);
                }
            }
            MapsGameObjects = new List<GameObject>();
            UnloadedHeaders = gameManager.CustomMapBuilder.GenerateMapHeaders().GetEnumerator();
            CanLoadMore = true;
        }


        private void SelectMap(MapHeader header)
        {
            gameManager.CustomMapBuilder.LoadCustomMapLevel(header);
        }

        private void AddMapHeaderButton(MapHeader header)
        {
            var obj = Instantiate(ButtonPrefab, MapsRoot);
            var button = obj.GetComponent<CustomMapButton>();
            button.Header = header;
            button.UIButton.onClick.AddListener(() =>
            {
                SelectMap(header);
            });
            MapsGameObjects.Add(obj);
        }

        private void AddErrorMessage(GenerateMapHeaderResult result)
        {
            var obj = Instantiate(ErrorMessagePrefab, MapsRoot);
            var badge = obj.GetComponent<CustomMapLoadFailureBadge>();
            badge.Result = result;
            MapsGameObjects.Add(obj);
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

                var current = UnloadedHeaders.Current;
                if (current.Success)
                {
                    AddMapHeaderButton(UnloadedHeaders.Current.Data);
                }
                else
                {
                    AddErrorMessage(current);
                }
            }
        }
    }
}
