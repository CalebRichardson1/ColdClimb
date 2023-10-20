using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(Image))]
public class PlayerHealthUI : MonoBehaviour
{
    private Image healthBackGroundIMG => GetComponent<Image>();
    [SerializeField] private VideoPlayer heartRateVideoPlayer;
    [SerializeField] private VideoClip fineHeartRate, injuredHeartRate, criticalHeartRate;

    private Health playerHealth;

    private int fineStatusCutOff, injuredStatusCutOff, criticalStatusCutOff;

    private void OnEnable() {
        Health.IsPlayerAction += SetPlayerHealth;
    }
    
    private void OnDisable(){
        Health.IsPlayerAction -= SetPlayerHealth;
    }

    private void SetPlayerHealth(Health currentPlayerHealth){
        playerHealth = currentPlayerHealth;
        playerHealth.OnHealthChangedCallback += UpdateHealthUI;
        playerHealth.OnMaxHealthModifiedCallback += SetStatusCutoff;
        SetStatusCutoff();
    }

    private void OnDestroy() {
        if(playerHealth != null){
            playerHealth.OnMaxHealthModifiedCallback -= SetStatusCutoff;
            playerHealth.OnHealthChangedCallback -= UpdateHealthUI;
        }
    }

    private void SetStatusCutoff(){
        fineStatusCutOff = Mathf.RoundToInt(playerHealth.MaxHealth * 0.75f);
        injuredStatusCutOff = Mathf.RoundToInt(playerHealth.MaxHealth * 0.25f);
        criticalStatusCutOff = Mathf.RoundToInt(playerHealth.MaxHealth * 0.01f);

        UpdateHealthUI();
    }

    private void UpdateHealthUI(){
        switch(playerHealth.CurrentHealth){
            case int health when health >= fineStatusCutOff: 
            healthBackGroundIMG.color = Color.green;
            heartRateVideoPlayer.clip = fineHeartRate;
            heartRateVideoPlayer.playbackSpeed = 1f;
            break;
            case int health when health >= injuredStatusCutOff: 
            healthBackGroundIMG.color = Color.yellow;
            heartRateVideoPlayer.clip = injuredHeartRate;
            heartRateVideoPlayer.playbackSpeed = 0.75f;
            break;
            case int health when health >= criticalStatusCutOff: 
            healthBackGroundIMG.color = Color.red;
            heartRateVideoPlayer.clip = criticalHeartRate;
            heartRateVideoPlayer.playbackSpeed = 0.5f;
            break;
        }
    }
}
