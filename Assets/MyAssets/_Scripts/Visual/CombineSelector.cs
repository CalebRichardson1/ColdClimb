using ColdClimb.Global;
using UnityEngine;
using UnityEngine.UI;

namespace ColdClimb.UI{
    public class CombineSelector : MonoBehaviour{
        private RectTransform RectTransform => GetComponent<RectTransform>();
        private Image SelectorImage => GetComponent<Image>();
        private Transform MyTransform => transform;

        private float stopMovingDistanceFromTarget = 0.01f;
        private float selectorMoveSpeed = 45f;
        private bool isValid;

        private Transform currentSelectedObject;
        private RectTransform selectedRectTransform;

        private void OnEnable() => GameManager.OnGameStateChange += EvaulateState;
        private void OnDisable() => GameManager.OnGameStateChange -= EvaulateState;

        private void EvaulateState(GameState state){
            isValid = state == GameState.CombineItemScreen;
            SelectorImage.enabled = isValid;
        }

        private void Update() {
            if(!isValid) return;
            DetectCursorDistance();
        }

        public void SetTarget(Transform target){
            currentSelectedObject = target;
            selectedRectTransform = target.GetComponent<RectTransform>();
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

        private void DetectCursorDistance(){
            if(currentSelectedObject == null) return;
            if(Vector3.Distance(MyTransform.position, currentSelectedObject.position) > stopMovingDistanceFromTarget){
                MoveSelector();
            }  
        }
    }
}
