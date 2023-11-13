using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour, ILoadable{
        #region Variables
        public static Action<bool> OnSprintAction;

        [SerializeField] private Transform characterOrientation;
        private float walkingSpeed;
        private float runningSpeed;

        private Rigidbody CharacterRigidbody => GetComponent<Rigidbody>();
        private InputManager InputManager => ResourceLoader.InputManager;
        private PlayerData PlayerData => ResourceLoader.MainPlayerData;

        private float currentSpeed;
        private float xInput;
        private float yInput;

        private Vector3 moveDirection;

        private bool canMove;
        #endregion

        #region Setup
        private void OnEnable(){
            PlayerData.LoadValuesCallback += LoadData;
            GameManager.OnGameStateChange += (state) => canMove = state == GameState.MainGame;
        } 

        private void OnDisable(){
            PlayerData.LoadValuesCallback -= LoadData;
            GameManager.OnGameStateChange -= (state) => canMove = state == GameState.MainGame; 
        } 

        public void LoadData(){
            walkingSpeed = PlayerData.playerStats.walkSpeed;
            runningSpeed = PlayerData.playerStats.runSpeed; 

            currentSpeed = walkingSpeed;
        }

        private void Start(){
            InputManager.ReturnSprintAction().started += StartSprinting;
            InputManager.ReturnSprintAction().canceled += StopSprinting;
            CharacterRigidbody.freezeRotation = true;
        }

        private void OnDestroy(){
        InputManager.ReturnSprintAction().started -= StartSprinting;
        InputManager.ReturnSprintAction().canceled -= StopSprinting;
        }
        #endregion

        #region Base Movement
        private void Update(){
            if(!canMove) return;
            MoveInput();
            CharacterSpeedLimit();
            if(currentSpeed == runningSpeed) CheckValidSprint();
        }

        public void MoveInput(){
        xInput = InputManager.GetPlayerMovement().x;
        yInput = InputManager.GetPlayerMovement().y;
        }

        private void FixedUpdate(){
            if(!canMove) return;
            moveDirection = characterOrientation.forward * yInput + characterOrientation.right * xInput;
            CharacterRigidbody.AddForce(100f * currentSpeed * moveDirection.normalized, ForceMode.Force);
        }

        private void CharacterSpeedLimit(){ //make sure our velocity doesn't get added continuously
            Vector3 flatVel2 = new(CharacterRigidbody.velocity.x, 0f, CharacterRigidbody.velocity.z);

            if (flatVel2.magnitude > currentSpeed)
            {
                Vector3 limitedVel = flatVel2.normalized * currentSpeed;
                CharacterRigidbody.velocity = new Vector3(limitedVel.x, CharacterRigidbody.velocity.y, limitedVel.z);
            }
        }
        #endregion

        #region Sprinting
        private void StartSprinting(InputAction.CallbackContext context){
            if(yInput > 0.55){
                currentSpeed = runningSpeed;
                OnSprintAction?.Invoke(true);
            }
        }

        private void CheckValidSprint(){
            if(yInput < 0.55) StopSprint();
        }

        private void StopSprinting(InputAction.CallbackContext context) => StopSprint();

        private void StopSprint(){
            currentSpeed = walkingSpeed;
            OnSprintAction?.Invoke(false);
        }
        #endregion
    }
}
