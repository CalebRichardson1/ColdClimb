using System;
using ColdClimb.Global.SaveSystem;
using UnityEngine;

namespace ColdClimb.UI{
    public class MainMenuController : MonoBehaviour{
        public static Action OnButtonPress;
        public static Action<RectTransform> OnMenuLoad;

        [Header("Game Data")]
        [SerializeField] private GameData gameData;

        [Header("Menus")]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject optionMenu;
        [SerializeField] private GameObject loadMenu;
        [SerializeField] private GameObject slotMenu;

        [Header("Default Selected UI")]
        [SerializeField] private RectTransform defaultOptionUI;
        [SerializeField] private RectTransform defaultLoadUI;
        [SerializeField] private RectTransform defaultSlotUI;

        [Header("Main Menu Buttons")]
        [SerializeField] private RectTransform continueGameButton;
        [SerializeField] private RectTransform loadGameButton;
        [SerializeField] private RectTransform newGameButton;

        private Menu currentMenu;
        private RectTransform defaultButton;

        private void Awake(){
            gameData.LoadValuesCallback += ShowContinue;
            gameData.LoadValidFilesCallback += ShowLoad;
        }

        private void OnDestroy(){
            gameData.LoadValuesCallback -= ShowContinue;
            gameData.LoadValidFilesCallback -= ShowLoad;
        }

        private void ShowContinue(){
            continueGameButton.gameObject.SetActive(true);
            defaultButton = continueGameButton;

            LoadDefaultButton();
        }

        private void ShowLoad(){
            // LoadDefaultButton();
        }

        private void Start(){
            currentMenu = Menu.MAIN;
            ShowVisuals();
            continueGameButton.gameObject.SetActive(false);
            loadGameButton.gameObject.SetActive(false);

            defaultButton = newGameButton;
            LoadDefaultButton();

            // Delete Later
            TMP_AudioManager.Instance.PlayMenuMusic();
        }

        public void ShowMenu(int index){
            switch (index){
                case 0: currentMenu = Menu.MAIN;
                break;
                case 1: currentMenu = Menu.LOAD;
                break;
                case 2: currentMenu = Menu.OPTION;
                break;
                case 3: currentMenu = Menu.SLOT;
                break;
                default: Debug.Log("Not Valid Menu Index, returning...");
                    return;
            }
            ShowVisuals();
            SetDefaultButton();
            OnButtonPress?.Invoke();
        }

        public void ExitGame(){
            OnButtonPress?.Invoke();
            Application.Quit();
        }

        public void ContinueGame(){
            gameData.LoadContinueGame();
        } 

        private void SetDefaultButton(){
            switch (currentMenu){
                case Menu.OPTION: OnMenuLoad?.Invoke(defaultOptionUI);
                    break;
                case Menu.LOAD: OnMenuLoad?.Invoke(defaultLoadUI);
                    break;
                case Menu.SLOT: OnMenuLoad?.Invoke(defaultSlotUI);
                    break;
            }
        }

        public void LoadDefaultButton() => OnMenuLoad?.Invoke(defaultButton);

        private void ShowVisuals(){
            mainMenu.SetActive(currentMenu == Menu.MAIN);
            optionMenu.SetActive(currentMenu == Menu.OPTION);
            loadMenu.SetActive(currentMenu == Menu.LOAD);
            slotMenu.SetActive(currentMenu == Menu.SLOT);
        }

        private enum Menu{
            MAIN,
            OPTION,
            LOAD,
            SLOT
        }
    }
}

