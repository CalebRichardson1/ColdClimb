using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ColdClimb.Global;

namespace ColdClimb.UI{
    [RequireComponent(typeof(RectTransform))]
    public class MenuSelector : MonoBehaviour{
        public static MenuSelector Instance;
        [SerializeField, Range(1f, 50f)] private float selectorMoveSpeed = 25f;
        [SerializeField, Range(0.01f, 5f)] private float stopMovingDistanceFromTarget = 0.01f;

        private RectTransform RectTransform => GetComponent<RectTransform>();
        private Image SelectorImage => GetComponent<Image>();
        private EventSystem CurrentEventSystem => EventSystem.current;

        private Transform MyTransform => transform;

        private Transform currentSelectedObject;
        private Transform defaultSelectedObject;

        private RectTransform selectedRectTransform;

        private bool isValid;
        private void Awake() {
            if(Instance != null && Instance != this){
                Destroy(gameObject);
                return;
            } 

            Instance = this;
        }

        private void OnEnable() => GameManager.OnGameStateChange += EvaluateGameState;
        private void OnDisable() => GameManager.OnGameStateChange -= EvaluateGameState;

        public void SetDefaultSelectedObject(Transform defaultSelection){
            defaultSelectedObject = defaultSelection;

            AssignDefaultSelection();
        } 

        public void SetIsValid(bool state){
            isValid = state;

            //set the cursor visual to be the incoming state
            SelectorImage.enabled = isValid;
        }

        private void AssignDefaultSelection(){
            //if the default selected object has not been assigned yet, return
            if (defaultSelectedObject == null) return;

            //if we don't have a object selected then select the default object
            CurrentEventSystem.SetSelectedGameObject(defaultSelectedObject.gameObject);
            SetTarget(defaultSelectedObject);
        }

        private void EvaluateGameState(GameState gameState){
            if(gameState == GameState.MainMenu){
                MainMenuController.OnMenuLoad += SetDefaultSelectedObject;
            }

            // Yikes
            isValid = gameState == GameState.MainMenu || gameState == GameState.StatusScreen 
                                   || gameState == GameState.ContextScreen
                                   || gameState == GameState.PauseMenu || gameState == GameState.CombineItemScreen;
                                   
            //set the cursor visual to be enabled when we are in a menu
            SelectorImage.enabled = isValid;
        }

        private void DetectNewObjectSelected(){
            if(CurrentEventSystem.currentSelectedGameObject != null && currentSelectedObject == null){
                SetTarget(CurrentEventSystem.currentSelectedGameObject.transform);
                return;
            }

            if(currentSelectedObject == null) return;

            if(CurrentEventSystem.currentSelectedGameObject != currentSelectedObject.gameObject){
                SetTarget(CurrentEventSystem.currentSelectedGameObject.transform);
            }
        }

        private void DetectCursorDistance(){
            if(currentSelectedObject == null) return;

            if(Vector3.Distance(MyTransform.position, currentSelectedObject.position) > stopMovingDistanceFromTarget){
                MoveSelector();
            }  
        }

        public void SetTarget(Transform target){
            CurrentEventSystem.SetSelectedGameObject(target.gameObject);
            currentSelectedObject = target;
            selectedRectTransform = target.GetComponent<RectTransform>();
        }

        private void Update() {
            if(!isValid) return;
            DetectCursorDistance();
            DetectNewObjectSelected(); 
        }

        private void MoveSelector(){
            // move the cursor smoothly to the pos of the newly selected object
            MyTransform.position = Vector3.Lerp(MyTransform.position, currentSelectedObject.transform.position, selectorMoveSpeed * Time.deltaTime);

            // scale the cursor smoothly to the scale of the selected rect transform
            var horizontalLerp = Mathf.Lerp(RectTransform.rect.size.x, selectedRectTransform.rect.size.x, selectorMoveSpeed * Time.deltaTime);
            var verticalLerp = Mathf.Lerp(RectTransform.rect.size.y, selectedRectTransform.rect.size.y, selectorMoveSpeed * Time.deltaTime);

            // apply the lerp calculation to the rect transform
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalLerp);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalLerp);
        }
    }
}
