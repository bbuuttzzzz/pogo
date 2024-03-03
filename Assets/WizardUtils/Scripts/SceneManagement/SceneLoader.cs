using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace WizardUtils.SceneManagement
{
    public class SceneLoader
    {
        #region Constants
        public enum LoadStates
        {
            NotLoaded,
            Loaded,
            Loading,
            Unloading
        }
        /// <summary>
        /// wait this many seconds before unloading when marked not needed
        /// </summary>
        const float UnloadDelaySeconds = 5f;
        #endregion

        private GameManager Parent;
        public readonly Scene Scene;

        public float TaskProgress { get; private set; }
        private Coroutine ActiveCoroutine;
        private bool CanCancelActiveCoroutine;
        public bool AllowSceneActivation;

        public LoadStates CurrentLoadState;
        public bool CurrentlyNeeded { get; private set; }

        public UnityEvent OnIdle;
        public UnityEvent OnReadyToActivate;
        public bool IsIdle => CurrentLoadState == LoadStates.Loaded || CurrentLoadState == LoadStates.NotLoaded;

        public SceneLoader(GameManager parent, Scene scene, bool currentlyLoaded)
        {
#if UNITY_WEBGL
            // don't be coy with scene loading on web
            AllowSceneActivation = true;
#endif
            if (scene == null)
            {
                throw new NullReferenceException("Tried to create a SceneLoader with a NULL scene");
            }

            Parent = parent;
            Scene = scene;
            CurrentLoadState = currentlyLoaded ? LoadStates.Loaded : LoadStates.NotLoaded;
            OnIdle = new UnityEvent();
            OnReadyToActivate = new UnityEvent();
        }

        public void MarkNeeded()
        {
            CurrentlyNeeded = true;
            if (CurrentLoadState == LoadStates.NotLoaded)
            {
                Load();
            }
            else if (CurrentLoadState == LoadStates.Unloading
                && CanCancelActiveCoroutine
                && ActiveCoroutine != null)
            {
                Parent.StopCoroutine(ActiveCoroutine);
                TaskProgress = 1;
                CurrentLoadState = LoadStates.Loaded;
            }
        }

        public void MarkNotNeeded(bool unloadInstantly = false)
        {
            CurrentlyNeeded=false;
            AllowSceneActivation = true;
            if (CurrentLoadState == LoadStates.Loaded)
            {
                Unload(unloadInstantly);
            }
        }


        #region Internal State Management

        private void Load()
        {
            if (CurrentLoadState != LoadStates.NotLoaded) throw new InvalidOperationException($"Tried to load LevelScene while in bad state {CurrentLoadState}");

            CurrentLoadState = LoadStates.Loading;
            ActiveCoroutine = Parent.StartCoroutine(LoadAsync());
        }

        private void Unload(bool instant = false)
        {
            if (CurrentLoadState != LoadStates.Loaded) throw new InvalidOperationException($"Tried to load LevelScene while in bad state {CurrentLoadState}");

            CurrentLoadState = LoadStates.Unloading;
            ActiveCoroutine = Parent.StartCoroutine(UnloadAsync(instant ? 0 : UnloadDelaySeconds));
        }

        private void FinishLoading()
        {
            CurrentLoadState = LoadStates.Loaded;

            if (!CurrentlyNeeded)
            {
                Unload();
            }
            else
            {
                OnIdle.Invoke();
            }
        }

        private void FinishUnloading()
        {
            CurrentLoadState = LoadStates.NotLoaded;

            if (CurrentlyNeeded)
            {
                Load();
            }
            else
            {
                OnIdle.Invoke();
            }
        }


        #endregion

        private IEnumerator UnloadAsync(float delaySeconds)
        {
            if (delaySeconds > 0)
            {
                TaskProgress = 0;
                CanCancelActiveCoroutine = true;
                yield return new WaitForSecondsRealtime(delaySeconds);
            }

            CanCancelActiveCoroutine = false;
            AsyncOperation unloadTask = null;
            try
            {
                unloadTask = SceneManager.UnloadSceneAsync(Scene.buildIndex);
            }
            catch(ArgumentException e)
            {
                Debug.LogError($"Tried to unload an invalid scene: {Scene.name} (buildIndex {Scene.buildIndex}). Giving up.");
            }

            if (unloadTask == null)
            {
                CurrentLoadState = LoadStates.NotLoaded;
                TaskProgress = 1;
                yield break;
            }

            while (true)
            {
                TaskProgress = unloadTask.isDone ? 1 : unloadTask.progress;

                if (unloadTask.isDone && !SceneIsLoaded(Scene))
                {
                    break;
                }

                yield return new WaitForSecondsRealtime(0.02f);
            }

            FinishUnloading();
        }

        private IEnumerator LoadAsync()
        {
            CanCancelActiveCoroutine = false;
            AsyncOperation loadTask = SceneManager.LoadSceneAsync(Scene.buildIndex, LoadSceneMode.Additive);
            loadTask.allowSceneActivation = AllowSceneActivation;

            while (true)
            {
                TaskProgress = loadTask.isDone ? 1 : loadTask.progress;

                if (loadTask.isDone || loadTask.progress >= 0.9f)
                {
                    break;
                }
                yield return new WaitForSecondsRealtime(0.02f);
            }

            if (CurrentlyNeeded)
            {
                OnReadyToActivate.Invoke();
                while (!AllowSceneActivation)
                {
                    yield return new WaitForSecondsRealtime(0.02f);
                }
            }

            loadTask.allowSceneActivation = true;

            while (!loadTask.isDone)
            {
                yield return new WaitForSecondsRealtime(0.02f);
            }

            FinishLoading();
        }

        private static bool SceneIsLoaded(Scene scene)
        {
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                if (SceneManager.GetSceneAt(n).buildIndex == scene.buildIndex)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            string levelName = Scene != null ? Scene.name : "NULL LEVEL";
            return $"LOADER {levelName} | {CurrentLoadState}";
        }
    }
}
