using Pogo;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pogo.Saving
{
    public class SavesScreenController : MonoBehaviour
    {
        public PogoMainMenuController parent;
        private Animator animator;

        private SaveSlotIds SelectedSlot;
        private DifficultyDescriptor SelectedDifficulty;

        public enum States
        {
            FileSelect,
            DifficultySelect
        }

        private States currentState = States.FileSelect;
        public States CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                switch(value)
                {
                    case States.FileSelect:
                        animator.SetTrigger("OpenFileSelect");
                        break;
                    case States.DifficultySelect:
                        animator.SetTrigger("OpenDifficultySelect");
                        break;
                }
            }
        }

        public GameObject FileSelectSubMenuParent;
        public GameObject NewGameSubMenuParent;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private bool setup = false;
        private void OnEnable()
        {
            if (!setup) SetupSaveFiles();
        }

        public void Back()
        {
            if (CurrentState == States.FileSelect)
            {
                parent.OpenHomeScreen();
            }
            else if (CurrentState == States.DifficultySelect)
            {
                CurrentState = States.FileSelect;
            }
        }

        public void NewGame(SaveSlotIds slotId)
        {
            SelectedSlot = slotId;

            SetupDifficulties();
            CurrentState = States.DifficultySelect;
        }

        public void LoadFile(SaveSlotIds slotId)
        {
            PogoGameManager.PogoInstance.LoadSlot(slotId);
            throw new NotImplementedException();
        }

        public void DeleteFile(SaveSlotIds slotId)
        {
            PogoGameManager.PogoInstance.DeleteSlot(slotId);

            var newObject = CreateSaveFileBox(slotId);
            newObject.transform.SetSiblingIndex(SaveSlotConstants.SaveSlotIdToIndex(slotId));
        }

        #region FileSelect
        public Transform SaveFilesParent;
        public GameObject LoadFilePrefab;
        public GameObject NewFilePrefab;

        public void SetupSaveFiles()
        {
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
        private void SetupDifficulties()
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
            throw new NotImplementedException();
        }
        #endregion

        #region NewGame
        TextMeshProUGUI PlaceholderText;


        #endregion
    }
}