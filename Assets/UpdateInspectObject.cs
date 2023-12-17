using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using UnityEngine;

namespace ColdClimb.Interactable{
    public class UpdateInspectObject : MonoBehaviour, ILoadable{
        public string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid(){
            id = System.Guid.NewGuid().ToString();
        }
        [SerializeField] private GameObject beforeInspectObject;
        [SerializeField] private GameObject afterInspectObject;

        private ScenesData ScenesData => ResourceLoader.ScenesData;

        private bool isSetAfter;

        private void Awake() {
            GameDataHandler.OnSaveInjectionCallback += SaveState;
            ScenesData.LoadValuesCallback += LoadData;
        }

        private void OnDestroy(){
            GameDataHandler.OnSaveInjectionCallback -= SaveState;
            ScenesData.LoadValuesCallback -= LoadData;
        }

        private void SaveState(){
            if(ScenesData.CurrentSceneData.InspectObjectsUpdated.ContainsKey(id)){
                ScenesData.CurrentSceneData.InspectObjectsUpdated.Remove(id);
            }
            ScenesData.CurrentSceneData.InspectObjectsUpdated.Add(id, isSetAfter);
        }

        public void LoadData(){
            ScenesData.CurrentSceneData.InspectObjectsUpdated.TryGetValue(id, out isSetAfter);
            if(isSetAfter){
                SetAfterInspect();
            }
        }

        public void SetAfterInspect(){
            beforeInspectObject.SetActive(false);
            afterInspectObject.SetActive(true);
            isSetAfter = true;
        }
    }
}
