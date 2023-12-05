using UnityEngine;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Global;

namespace ColdClimb.Camera{

    public class FirstPersonCameraController : MonoBehaviour, ILoadable{
        [SerializeField] private Transform camHolder;
        [SerializeField] private Transform characterOrientation;

        private InputManager InputManager => ResourceLoader.InputManager;
        private PlayerData PlayerData => ResourceLoader.MainPlayerData;
        private OptionsData OptionsData => SystemManager.Instance.OptionsData;

        public float XValue { get; private set; }
        public float YValue { get; private set; }

        private bool canLook = true;
        private float CameraMoveSens => OptionsData.settings.lookSensitivity;

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

        private void Update(){
            LookInput();
        }

        public void LookInput(){
            if(!canLook) return;
            Vector2 inputVector = CameraMoveSens * Time.deltaTime * InputManager.GetLookDelta();
            XValue += inputVector.x;
            YValue -= inputVector.y;
            YValue = Mathf.Clamp(YValue, -80f, 80f);
        }

        private void LateUpdate(){
            UpdateCamera();
        }

        private void UpdateCamera(){
            characterOrientation.transform.localRotation = Quaternion.Euler(0, XValue, 0);
            camHolder.localRotation = Quaternion.Euler(YValue, XValue, 0);
        }
    }
}

