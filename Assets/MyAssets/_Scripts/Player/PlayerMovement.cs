using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColdClimb.Player{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour, ILoadable{
        public static Action<bool> OnSprintAction;

        [Header("Slope Variables")]
        [SerializeField] private float groundDetectionRange = 4f;

        [Header("Sprint Variables")]
        [SerializeField] private float sprintTimer;

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
        private WaitForSeconds sprintWaitTimer;

        private bool canMove;
        private bool canSprint = true;

        private void Awake(){
            PlayerData.LoadValuesCallback += LoadData;
            GameManager.OnGameStateChange += EvaluateGameState;
            sprintWaitTimer = new WaitForSeconds(sprintTimer);
        } 

        private void Start(){
            InputManager.ReturnSprintAction().started += StartSprinting;
            InputManager.ReturnSprintAction().canceled += StopSprinting;
            CharacterRigidbody.freezeRotation = true;
        }

        private void OnDestroy(){
            PlayerData.LoadValuesCallback -= LoadData;
            GameManager.OnGameStateChange -= EvaluateGameState;
            InputManager.ReturnSprintAction().started -= StartSprinting;
            InputManager.ReturnSprintAction().canceled -= StopSprinting;
            StopAllCoroutines();
        } 

        public void LoadData(){
            walkingSpeed = PlayerData.playerStats.walkSpeed;
            runningSpeed = PlayerData.playerStats.runSpeed; 

            currentSpeed = walkingSpeed;
        }

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
            SlopeDetection();
            moveDirection = characterOrientation.forward * yInput + characterOrientation.right * xInput;
            CharacterRigidbody.AddForce(100f * currentSpeed * moveDirection.normalized, ForceMode.Force);
        }

        private void SlopeDetection(){
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundDetectionRange)){
                var slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if(slopeAngle > 0){
                    // Makes the player follow the slope for a smooth transition rather then a bumpy one
                    CharacterRigidbody.velocity = Vector3.ProjectOnPlane(CharacterRigidbody.velocity, hit.normal);
                }
            }
        }

        private void CharacterSpeedLimit(){ //make sure our velocity doesn't get added continuously
            Vector3 flatVel2 = new(CharacterRigidbody.velocity.x, 0f, CharacterRigidbody.velocity.z);

            if (flatVel2.magnitude > currentSpeed){
                Vector3 limitedVel = flatVel2.normalized * currentSpeed;
                CharacterRigidbody.velocity = new Vector3(limitedVel.x, CharacterRigidbody.velocity.y, limitedVel.z);
            }
        }

        private void StartSprinting(InputAction.CallbackContext context){
            if(!canSprint) return;
            if(yInput > 0.55){
                currentSpeed = runningSpeed;
                OnSprintAction?.Invoke(true);
                StartCoroutine(SprintCooldownCoroutine());
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
        
        private IEnumerator SprintCooldownCoroutine(){
            canSprint = false;
            yield return sprintWaitTimer;
            canSprint = true;
        }

        private void EvaluateGameState(GameState state){
            canMove = state == GameState.MainGame;
            if(state == GameState.MainGame){
                CharacterRigidbody.constraints = RigidbodyConstraints.None;
                CharacterRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            else{
                CharacterRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.down * groundDetectionRange);  
        }
    }
}
