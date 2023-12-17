using System;
using System.Collections;
using ColdClimb.Global;
using UnityEngine;

namespace ColdClimb.UI{
    public class ScreenFader : MonoBehaviour{
        private bool isFading = false;

        private CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();

        private void Awake() {
            GameManager.OnGameStateChange += EvaluateGameState;
        }

        private void OnDestroy() {
            GameManager.OnGameStateChange -= EvaluateGameState;
        }

        private void EvaluateGameState(GameState state){
            if(state == GameState.GameOver){
                StopAllCoroutines();
                CanvasGroup.alpha = 0;
                isFading = false;
            }
        }


        public void FadeToAndFromBlack(float duration, Action afterFadedBlackCallback){
            if(isFading) return;
            
            StartCoroutine(FadeToAndFromBlackCoroutine(duration, afterFadedBlackCallback));
        }

        public void FadeToBlack(float duration, Action afterFadedCallback){
            if(isFading) return;

            StartCoroutine(FadeToBlackCoroutine(duration, afterFadedCallback));
        }

        public void FadeFromBlack(float duration, Action afterFadedCallback){
            if(isFading) return;
            
            StartCoroutine(FadeFromBlackCoroutine(duration, afterFadedCallback));
        }

        private IEnumerator FadeToBlackCoroutine(float duration, Action afterFadedCallback){
            isFading = true;

            CanvasGroup.alpha = 0;

            //fade to black
            while(CanvasGroup.alpha < 1){
                CanvasGroup.alpha = CanvasGroup.alpha + (Time.deltaTime / duration);
                yield return null;
            }

            CanvasGroup.alpha = 1f;

            afterFadedCallback?.Invoke();
            isFading = false;
        }

        private IEnumerator FadeFromBlackCoroutine(float duration, Action afterFadedCallback){
            isFading = true;

            CanvasGroup.alpha = 1;

            //fade from black
            while(CanvasGroup.alpha > 0){
                CanvasGroup.alpha = CanvasGroup.alpha - (Time.deltaTime / duration);
                yield return null;
            }

            CanvasGroup.alpha = 0f;
            afterFadedCallback?.Invoke();
            isFading = false;
        }

        private IEnumerator FadeToAndFromBlackCoroutine(float duration, Action afterFadedBlackCallback){
            isFading = true;

            //fade to black
            while(CanvasGroup.alpha < 1){
                CanvasGroup.alpha = CanvasGroup.alpha + (Time.deltaTime / duration);
                yield return null;
            }

            CanvasGroup.alpha = 1f;

            afterFadedBlackCallback?.Invoke();

            //fade from black
            while(CanvasGroup.alpha > 0){
                CanvasGroup.alpha = CanvasGroup.alpha - (Time.deltaTime / duration);
                yield return null;
            }

            CanvasGroup.alpha = 0f;
            isFading = false;
        }

    } 
}
