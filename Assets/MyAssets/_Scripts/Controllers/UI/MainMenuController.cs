using System;
using SaveLoadSystem;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static Action OnButtonPress;

    [Header("Game Data")]
    [SerializeField] private GameData gameData;

    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject loadMenu;
    [SerializeField] private GameObject slotMenu;

    [Header("Menu Buttons")]
    [SerializeField] private GameObject continueGameButton;
    [SerializeField] private GameObject loadGameButton;

    private Menu currentMenu;

    private void Awake() {
        gameData.LoadValuesCallback += ShowContinue;
        gameData.LoadValidFilesCallback += ShowLoad;
    }

    private void OnDestroy() {
        gameData.LoadValuesCallback -= ShowContinue;
        gameData.LoadValidFilesCallback -= ShowLoad;
    }

    private void ShowContinue(){
        continueGameButton.SetActive(true);
    }

    private void ShowLoad(){
        // loadGameButton.SetActive(true);
    }

    private void Start() {
        currentMenu = Menu.MAIN;
        ShowVisuals();
        continueGameButton.SetActive(false);
        loadGameButton.SetActive(false);

        // Delete Later
        TMP_AudioManager.Instance.PlayMenuMusic();
    }

    public void ShowMainMenu(){
        currentMenu = Menu.MAIN;
        ShowVisuals();
        OnButtonPress?.Invoke();
    }

    public void ShowLoadMenu(){
        currentMenu = Menu.LOAD;
        ShowVisuals();
        OnButtonPress?.Invoke();
    }

    public void ShowOptionsMenu(){
        currentMenu = Menu.OPTION;
        ShowVisuals();
        OnButtonPress?.Invoke();
    }

    public void ShowSlotMenu(){
        currentMenu = Menu.SLOT;
        ShowVisuals();
        OnButtonPress?.Invoke();
    }

    public void ExitGame(){
        OnButtonPress?.Invoke();
        Application.Quit();
    }

    public void ContinueGame(){
        gameData.LoadContinueGame();
    } 

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

