using ColdClimb.Generic;
using ColdClimb.Global;
using ColdClimb.Global.LevelSystem;
using ColdClimb.Global.SaveSystem;
using UnityEngine;

namespace ColdClimb.UI{
    public class GameOverController : MonoBehaviour{
        [SerializeField] GameData gameData;
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private Transform continueButton;
        [SerializeField] private Transform defaultSelect;

        private Health playerHealth;

        private void Awake() {
            Health.IsPlayerAction += SubscribeToDeath;
            SceneDirector.LoadedScene += DetectScene;
        }

        private void OnDestroy(){
            Health.IsPlayerAction -= SubscribeToDeath;
            playerHealth.OnDieCallback -= OnPlayerDieEvent;
            SceneDirector.LoadedScene -= DetectScene;
        }

        private void Start() {
            gameOverUI.SetActive(false);
        }

        public void DetectScene(SceneIndex scene){
            if(scene == SceneIndex.MAIN_MENU){
                gameOverUI.SetActive(false);
            }
        }

        public void ContinueGame(){
            gameData.LoadContinueGame();
        }

        public void LoadMainMenu(){
            GameManager.UpdateGameState(GameState.MainMenu);
        }

        private void SubscribeToDeath(Health health){
            playerHealth = health;
            health.OnDieCallback += OnPlayerDieEvent;
        }

        private void OnPlayerDieEvent(){
            GameManager.UpdateGameState(GameState.GameOver);
            GlobalUIReference.ScreenFader.FadeToAndFromBlack(3f, ShowGameOverUI);
        }

        private void ShowGameOverUI(){
            //Animation
            gameOverUI.SetActive(true);
            MenuSelector.Instance.SetDefaultSelectedObject(defaultSelect);
            MenuSelector.Instance.SetIsValid(true);
        }
    }
}
