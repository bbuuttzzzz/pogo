using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.Saving
{
    public class SavesScreenController : MonoBehaviour
    {
        public PogoMainMenuController parent;
        private Animator animator;

        private SaveSlotIds SelectedSlot;
        public DifficultyDescriptor SelectedDifficulty;
        private string DisplayName;

        public enum States
        {
            FileSelect,
            DifficultySelect,
            NewGameFinalize
        }

        private States currentState = States.FileSelect;
        public States CurrentState
        {
            get => currentState;
            set
            {
                if (CurrentState == value) return;
                currentState = value;
                switch(value)
                {
                    case States.FileSelect:
                        RefreshFileSelect();
                        animator.SetTrigger("OpenFileSelect");
                        break;
                    case States.DifficultySelect:
                        RefreshDifficultySelect();
                        animator.SetTrigger("OpenDifficultySelect");
                        break;
                    case States.NewGameFinalize:
                        RefreshNewGameFinalize();
                        animator.SetTrigger("OpenNewGameFinalize");
                        break;
                }
            }
        }

        public GameObject FileSelectSubMenuParent;
        public GameObject NewGameSubMenuParent;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            ChangeDifficultyButton.OnPressed.AddListener(OpenDifficultySelect);
        }

        private void OnEnable()
        {
            RefreshFileSelect();
        }

        private void OnDisable()
        {
            CurrentState = States.FileSelect;
            FilesLoaded = false;
        }

        private void OnValidate()
        {
            if (SelectedDifficulty == null)
            {
                Debug.LogError("SelectedDifficulty should be the default difficulty (probably normal)", this);
            }
        }

        public void Back()
        {
            if (CurrentState == States.FileSelect)
            {
                parent.OpenHomeScreen();
            }
            else if (CurrentState == States.DifficultySelect)
            {
                CurrentState = States.NewGameFinalize;
            }
            else if (currentState == States.NewGameFinalize)
            {
                CurrentState = States.FileSelect;
            }
        }

        public void NewGame(SaveSlotIds slotId)
        {
            SelectedSlot = slotId;
            CurrentState = States.NewGameFinalize;
            DisplayName = "";
        }

        public void LoadFile(SaveSlotIds slotId)
        {
            PogoGameManager.PogoInstance.LoadSlot(slotId);
            parent.OpenWorldScreen();
        }

        public void DeleteFile(SaveSlotIds slotId)
        {
            PogoGameManager.PogoInstance.DeleteSlot(slotId);

            var oldSaveFileElement = SaveFilesParent.GetChild(SaveSlotConstants.SaveSlotIdToIndex(slotId));
            Destroy(oldSaveFileElement.gameObject);
            var newObject = CreateSaveFileBox(slotId);
            newObject.transform.SetSiblingIndex(SaveSlotConstants.SaveSlotIdToIndex(slotId));
        }

        #region FileSelect
        public Transform SaveFilesParent;
        public GameObject LoadFilePrefab;
        public GameObject NewFilePrefab;
        private bool FilesLoaded = false;

        public void RefreshFileSelect()
        {
            if (FilesLoaded) return;
            FilesLoaded = true;

            // destroy existing things
            int childCount = SaveFilesParent.childCount;
            for (int n = 0; n < childCount; n++)
            {
                Destroy(SaveFilesParent.GetChild(n).gameObject);
            }

            for (int n = 0; n < SaveSlotConstants.SaveSlotCount; n++)
            {
                SaveSlotIds slotId = SaveSlotConstants.SaveSlotIdFromIndex(n);
                _ = CreateSaveFileBox(slotId);
            }
        }

        private GameObject CreateSaveFileBox(SaveSlotIds slotId)
        {
            var previewData = PogoGameManager.PogoInstance.PreviewSlot(slotId);

            GameObject newObject;
            if (previewData.HasValue)
            {
                newObject = Instantiate(LoadFilePrefab, SaveFilesParent);
                var controller = newObject.GetComponent<SaveFileLoadBoxController>();
                controller.SetData(slotId, previewData.Value);
                controller.OnLoadTriggered.AddListener(LoadFile);
                controller.OnDeleteTriggered.AddListener(DeleteFile);
            }
            else
            {
                newObject = Instantiate(NewFilePrefab, SaveFilesParent);
                var controller = newObject.GetComponent<SaveFileNewBoxController>();
                controller.SlotId = slotId;
                controller.OnNewGameTriggered.AddListener(NewGame);
            }

            return newObject;
        }

        #endregion

        #region Difficulties
        public Transform DifficultiesParent;
        public GameObject DifficultyButtonPrefab;

        private bool difficultiesSetUp = false;
        private void RefreshDifficultySelect()
        {
            if (difficultiesSetUp) return;
            difficultiesSetUp = true;


            // destroy existing things
            int childCount = DifficultiesParent.childCount;
            for (int n = 0; n < childCount; n++)
            {
                Destroy(DifficultiesParent.GetChild(n).gameObject);
            }

            foreach (var difficulty in PogoGameManager.PogoInstance.DifficultyManifest.Difficulties)
            {
                GameObject newObject = Instantiate(DifficultyButtonPrefab, DifficultiesParent);
                var controller = newObject.GetComponent<DifficultyButtonController>();
                controller.SetDifficulty(difficulty);
                controller.OnDifficultySelected.AddListener(SelectDifficulty);
            }
        }

        public void SelectDifficulty(DifficultyDescriptor difficulty)
        {
            SelectedDifficulty = difficulty;
            Back();
        }
        #endregion

        #region NewGameFinalize
        public TextMeshProUGUI NewGameNamePlaceholderText;
        public TMP_InputField NewGameNameInputField;

        public Button FinishButton;
        public ChangeDifficultyButtonController ChangeDifficultyButton;

        private bool NewGameFinalizeInitialized;
        public void RefreshNewGameFinalize()
        {
            NewGameNamePlaceholderText.text = SaveSlotConstants.SaveSlotName(SelectedSlot);
            ChangeDifficultyButton.SetDifficulty(SelectedDifficulty);
            NewGameNameInputField.text = DisplayName;

            if (NewGameFinalizeInitialized) return;
            NewGameFinalizeInitialized = true;

            NewGameNameInputField.onValueChanged.AddListener(OnNameChanged);
        }

        private void OnNameChanged(string arg0)
        {
            DisplayName = arg0;
        }

        private void OpenDifficultySelect()
        {
            CurrentState = States.DifficultySelect;
        }

        public void FinishNewGame()
        {
            string finalName = string.IsNullOrEmpty(DisplayName) ? SaveSlotConstants.SaveSlotName(SelectedSlot) : DisplayName;

            PogoGameManager.PogoInstance.NewGameSlot(SelectedSlot, SelectedDifficulty, finalName);
            LoadFile(SelectedSlot);

        }
        #endregion
    }
}