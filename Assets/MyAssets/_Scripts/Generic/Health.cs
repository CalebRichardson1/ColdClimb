using System;
using UnityEngine;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Global;

namespace ColdClimb.Generic{
    public class Health : MonoBehaviour, ILoadable{
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        [SerializeField] private bool isPlayer;
        [SerializeField] private int currentHealth;
        [SerializeField] private int maxHealth;

        public Action OnHealthChangedCallback;
        public static Action<Health> IsPlayerAction;
        public Action OnDieCallback;
        public Action OnMaxHealthModifiedCallback;

        PlayerData PlayerData => ResourceLoader.MainPlayerData;

        private void OnEnable(){
            if(isPlayer){
                PlayerData.LoadValuesCallback += LoadData;
                OnHealthChangedCallback += UpdatePlayerData;
                OnMaxHealthModifiedCallback += UpdatePlayerData;
            }
        }

        private void OnDisable(){
            if(isPlayer){
                PlayerData.LoadValuesCallback -= LoadData;
                OnHealthChangedCallback -= UpdatePlayerData;
                OnMaxHealthModifiedCallback -= UpdatePlayerData;
            }
        }

        private void UpdatePlayerData(){
            PlayerData.playerStats.currentHealth = currentHealth;
            PlayerData.playerStats.maxHealth = maxHealth;
        }

        public void LoadData(){
            currentHealth = PlayerData.playerStats.currentHealth;
            maxHealth = PlayerData.playerStats.maxHealth;
            IsPlayerAction?.Invoke(this);
        }

        public void TakeDamage(int amount){
            currentHealth -= amount;
            if(currentHealth <= 0){
                if(isPlayer){
                    OnDieCallback?.Invoke();
                    return;
                }
                else{
                    // Use the action for the future for non player objects as well, this is just for testing/implementing
                    Destroy(gameObject);
                    return;
                }
            }

            OnHealthChangedCallback?.Invoke();
        }

        public void HealHealth(int amount){
            currentHealth += amount;
            if(currentHealth > maxHealth){
                currentHealth = maxHealth;
            }

            OnHealthChangedCallback?.Invoke();
        }

        public void ModifyMaxHealth(int amount){
            maxHealth += amount;
            if(isPlayer){
                OnMaxHealthModifiedCallback?.Invoke();
            }
        }

        public void SetupHealth(int amount){
            maxHealth = amount;
            currentHealth = maxHealth;
        }
    }
}
    