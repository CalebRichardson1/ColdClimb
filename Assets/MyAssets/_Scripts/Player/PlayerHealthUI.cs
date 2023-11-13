using ColdClimb.Generic;
using ColdClimb.Item;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ColdClimb.Player{

    [RequireComponent(typeof(Image))]
    public class PlayerHealthUI : MonoBehaviour{
        public bool UnlockedHealthBar {get; private set;}
        [SerializeField] private VideoPlayer heartRateVideoPlayer;
        [SerializeField] private VideoClip fineHeartRate, injuredHeartRate, criticalHeartRate;
        [Header("HealthBar")]
        [SerializeField] private Image HealthBarIMG;
        [SerializeField] private float fillSpeed = 3f;

        private Image HealthBackGroundIMG => GetComponent<Image>();
        
        private Health playerHealth;
        private int fineStatusCutOff, injuredStatusCutOff, criticalStatusCutOff;
        private float healthBarTarget, current, target;
        private bool moveHealthBar = false;

        private void OnEnable(){
            Health.IsPlayerAction += SetPlayerHealth;
            ModiferItem.EventToTrigger += UnlockHealthBar; //change later
        }
        
        private void OnDisable(){
            Health.IsPlayerAction -= SetPlayerHealth;
            ModiferItem.EventToTrigger -= UnlockHealthBar; //change later
        }

        private void Start(){
            if(!UnlockedHealthBar) HealthBarIMG.gameObject.SetActive(false);
        }

        private void Update(){
            if(UnlockedHealthBar && moveHealthBar){
                LerpHealthBar();
            }
        }

        private void SetPlayerHealth(Health currentPlayerHealth){
            playerHealth = currentPlayerHealth;
            playerHealth.OnHealthChangedCallback += UpdateHealthUI;
            playerHealth.OnMaxHealthModifiedCallback += SetStatusCutoff;
            SetStatusCutoff();
        }

        private void OnDestroy(){
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
                if(UnlockedHealthBar) UpdateHealthBar(health, HealthState.Healthy);
                HealthBackGroundIMG.color = Color.green;
                heartRateVideoPlayer.clip = fineHeartRate;
                heartRateVideoPlayer.playbackSpeed = 1f;
                break;
                case int health when health >= injuredStatusCutOff: 
                if(UnlockedHealthBar) UpdateHealthBar(health, HealthState.Injured);
                HealthBackGroundIMG.color = Color.yellow;
                heartRateVideoPlayer.clip = injuredHeartRate;
                heartRateVideoPlayer.playbackSpeed = 0.75f;
                break;
                case int health when health >= criticalStatusCutOff: 
                if(UnlockedHealthBar) UpdateHealthBar(health, HealthState.Critical);
                HealthBackGroundIMG.color = Color.red;
                heartRateVideoPlayer.clip = criticalHeartRate;
                heartRateVideoPlayer.playbackSpeed = 0.5f;
                break;
            }
        }

        private void UpdateHealthBar(int health, HealthState state){
            switch (state){
                case HealthState.Healthy: HealthBarIMG.color = Color.green;
                    break;
                case HealthState.Injured: HealthBarIMG.color = Color.yellow;
                    break;
                case HealthState.Critical: HealthBarIMG.color = Color.red;
                    break;
            }
            //lerp for smooth animation
            healthBarTarget = (float)health / playerHealth.MaxHealth;
            target = 1;
            moveHealthBar = true;
        }

        private void LerpHealthBar(){
            current = Mathf.MoveTowards(current, target, fillSpeed * Time.deltaTime);
            HealthBarIMG.fillAmount = Mathf.Lerp(HealthBarIMG.fillAmount, healthBarTarget, current);
            //if close enough snap to the target
            if(Mathf.Abs(HealthBarIMG.fillAmount - healthBarTarget) <= 0.005f){
                target = 0;
                current = 0;
                HealthBarIMG.fillAmount = healthBarTarget;
                moveHealthBar = false;
            } 
        }

        public void UnlockHealthBar(bool unlocked){
            HealthBarIMG.gameObject.SetActive(unlocked);
            UnlockedHealthBar = unlocked;
            UpdateHealthUI();
        }

        private enum HealthState{
            Healthy,
            Injured,
            Critical
        }
    }
}

