using System;
using UnityEngine;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Global;

namespace ColdClimb.Camera{

    public class FirstPersonCameraController : MonoBehaviour, ILoadable{
        #region Variables
        [SerializeField] private Transform camHolder;
        [SerializeField] private Transform characterOrientation;
        [SerializeField, Range(0.1f, 50f)] private float cameraMoveSens = 25f;

        private InputManager InputManager => ResourceLoader.InputManager;
        private PlayerData PlayerData => ResourceLoader.MainPlayerData;

        public float XValue { get; private set; }
        public float YValue { get; private set; }

        private bool canLook = true;


        private void Awake(){
            PlayerData.LoadValuesCallback += LoadData;
            GameManager.OnGameStateChange += (state) => canLook = state == GameState.MainGame;
        } 

        private void OnDestroy(){
            PlayerData.LoadValuesCallback -= LoadData;
            GameManager.OnGameStateChange -= (state) => canLook = state == GameState.MainGame;
        }

        public void LoadData(){
            XValue = PlayerData.playerLocation.playerLookX;
            YValue = PlayerData.playerLocation.playerLookY;      
        }
        #endregion
        
        #region Input

        private void Update(){
            LookInput();
        }

        public void LookInput(){
            if(!canLook) return;
            Vector2 inputVector = cameraMoveSens * Time.deltaTime * InputManager.GetLookDelta();
            XValue += inputVector.x;
            YValue -= inputVector.y;
            YValue = Mathf.Clamp(YValue, -80f, 80f);
        }
        #endregion

        #region Updating Camera
        private void LateUpdate(){
            UpdateCamera();
        }

        private void UpdateCamera(){
            characterOrientation.transform.localRotation = Quaternion.Euler(0, XValue, 0);
            camHolder.localRotation = Quaternion.Euler(YValue, XValue, 0);
        }
        #endregion
    }
}

