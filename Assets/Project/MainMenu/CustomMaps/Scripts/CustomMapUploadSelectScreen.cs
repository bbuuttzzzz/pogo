using Pogo.CustomMaps.Indexing;
using System.Collections.Generic;
using UnityEngine;
using WizardUI;
using WizardUtils;

namespace Pogo.CustomMaps.UI
{
    public class CustomMapUploadSelectScreen : MonoBehaviour
    {
        private PogoGameManager gameManager;

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


        private void SelectMap(MapHeader header)
        {
            gameManager.CustomMapBuilder.LoadCustomMapLevel(header.FolderPath, $"{header.MapName}.bsp");
        }

        private void AddButton(MapHeader header)
        {
            var obj = Instantiate(ButtonPrefab, ButtonsRoot);
            var button = obj.GetComponent<CustomMapButton>();
            var scrollThrough = button.UIButton.gameObject.AddComponent<ScrollPassThrough>();
            scrollThrough.ParentScrollHandler = Scroller;
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
