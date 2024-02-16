using Pogo;
using Pogo.CustomMaps.Indexing;
using Pogo.CustomMaps.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizardUI;

public class CustomMapsScreenController : MonoBehaviour
{
    private PogoGameManager gameManager;
    public Transform ButtonsRoot;
    public GameObject ButtonPrefab;
    public LoadMoreScroller ButtonsScroller;
    public int LoadMoreCount = 5;

    private IEnumerator<MapHeader> UnloadedHeaders;
    private bool CanLoadMore;
    private List<CustomMapButton> Buttons;


    private void Awake()
    {
        gameManager = PogoGameManager.PogoInstance;
        ButtonsScroller.OnShouldLoadMore.AddListener(() => LoadMore(LoadMoreCount));
    }

    private void OnEnable()
    {
        ResetButtons();
    }

    private void ResetButtons()
    {
        if (Buttons != null)
        {
            foreach( var button in Buttons)
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
        gameManager.CustomMapBuilder.LoadCustomMapLevel(header.FolderPath, $"{header.MapName}.bsp");
    }

    private void AddButton(MapHeader header)
    {
        var obj = Instantiate(ButtonPrefab, ButtonsRoot);
        var button = obj.GetComponent<CustomMapButton>();
        button.Header = header;
        button.UIButton.onClick.AddListener(() =>
        {
            SelectMap(button.Header);
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
