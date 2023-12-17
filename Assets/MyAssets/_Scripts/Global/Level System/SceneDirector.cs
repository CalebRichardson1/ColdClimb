using System;
using System.Collections;
using System.Collections.Generic;
using ColdClimb.Global.SaveSystem;
using ColdClimb.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColdClimb.Global.LevelSystem{    

    public class SceneDirector : MonoBehaviour{
        public static SceneIndex CurrentSceneIndex;

        public static SceneDirector Instance;
        public static Action<SceneIndex> LoadedScene;

        private bool loadedScene = false;

        private List<AsyncOperation> scenesLoading = new();


        private SceneIndex nextSceneIndex = SceneIndex.NUMSCENEINDEX;
        private Action currentLoadedCallback;

        private void Awake(){
            Instance = this;
            GameDataHandler.LoadGameScene += LoadScene;
        }

        private void OnDestroy(){
            GameDataHandler.LoadGameScene -= LoadScene;
        }

        public void LoadScene(SceneIndex index, Action loadedCallback){
            if(CurrentSceneIndex == index){
                return;
            }

            GameManager.UpdateGameState(GameState.LoadingScene);
            currentLoadedCallback = loadedCallback;
            nextSceneIndex = index;

            if(loadedScene){
                GlobalUIReference.ScreenFader.FadeToBlack(0.5f, LoadingScenes);
                return;
            }

            LoadingScenes();
        }

        private void LoadingScenes(){
            if(loadedScene) scenesLoading.Add(SceneManager.UnloadSceneAsync((int)CurrentSceneIndex)); //unload the current scene if we have loaded one
            scenesLoading.Add(SceneManager.LoadSceneAsync((int)nextSceneIndex, LoadSceneMode.Additive)); //start loading the next scene
            loadedScene = true;
            CurrentSceneIndex = nextSceneIndex;

            StartCoroutine(GetSceneLoadProgress(currentLoadedCallback));
        }

        // TO-DO:switch to async/await method
        private IEnumerator GetSceneLoadProgress(Action loadedCallback){
            for (int i = 0; i < scenesLoading.Count; i++){
                while (!scenesLoading[i].isDone){
                    yield return null;
                }
            }
                scenesLoading.Clear();
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)CurrentSceneIndex));


                LoadedScene?.Invoke(CurrentSceneIndex);
                loadedCallback?.Invoke();

                nextSceneIndex = SceneIndex.NUMSCENEINDEX;
                currentLoadedCallback = null;

                GlobalUIReference.ScreenFader.FadeFromBlack(0.5f, null);
        }
    }
}
