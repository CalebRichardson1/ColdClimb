using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLevelLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(!other.gameObject.CompareTag("Player")) return;

        LoadNextLevel();
    }

    private void LoadNextLevel(){
        GameManager.UpdateGameState(GameState.SavingGame);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
