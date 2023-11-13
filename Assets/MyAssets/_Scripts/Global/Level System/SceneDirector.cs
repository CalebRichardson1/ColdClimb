using System;
using System.Collections;
using System.Collections.Generic;
using ColdClimb.Global.SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColdClimb.Global.LevelSystem{    

    public class SceneDirector : MonoBehaviour{
        public static SceneDirector Instance;

        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Image progressBar;

        private SceneIndex currentSceneIndex;
        private bool loadedScene = false;

        private List<AsyncOperation> scenesLoading = new();
        private float totalSceneProgress, target;

        private void Awake(){
            Instance = this;
            loadingScreen.SetActive(false);
            GameDataHandler.LoadGameScene += LoadScene;
        }

        private void OnDestroy(){
            GameDataHandler.LoadGameScene -= LoadScene;
        }

        public void LoadScene(SceneIndex index, Action loadedCallback){
            if(loadingScreen != null){
                loadingScreen.SetActive(true);
            }

            if(loadedScene)scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentSceneIndex)); //unload the current scene if we have loaded one
            scenesLoading.Add(SceneManager.LoadSceneAsync((int)index, LoadSceneMode.Additive)); //start loading the next scene
            loadedScene = true;
            currentSceneIndex = index;

            StartCoroutine(GetSceneLoadProgress(loadedCallback));
        }

        // TO-DO:switch to async/await method
        private IEnumerator GetSceneLoadProgress(Action loadedCallback){
            for (int i = 0; i < scenesLoading.Count; i++){
                while (!scenesLoading[i].isDone){
                    target = 0;
                    progressBar.fillAmount = 0;
                    totalSceneProgress = 0;

                    foreach (AsyncOperation operation in scenesLoading){
                        totalSceneProgress += operation.progress;
                    }

                    totalSceneProgress /= scenesLoading.Count;

                    target = totalSceneProgress;
                    yield return null;
                }
            }
                scenesLoading.Clear();
                loadingScreen.SetActive(false);
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)currentSceneIndex));

                loadedCallback?.Invoke();
        }

        private void Update(){
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, 3 * Time.deltaTime);
        }
    }
}
