using System;
using ColdClimb.Global;
using ColdClimb.Global.SaveSystem;
using UnityEngine;

namespace ColdClimb.UI{
    public class SlotMenuController : MonoBehaviour
    {
        public event Action<SaveSlot> OnSlotSelect;

        public void NewGame(){
            OnSlotSelect += GameDataHandler.NewGame;
        }

        public void LoadGame(){
            OnSlotSelect -= GameDataHandler.NewGame;
        }

        private void OnDestroy() {
            OnSlotSelect -= GameDataHandler.NewGame;
        }

        public void Slot1() => OnSlotSelect?.Invoke(SaveSlot.SLOT1);

        public void Slot2() => OnSlotSelect?.Invoke(SaveSlot.SLOT2);

        public void Slot3() => OnSlotSelect?.Invoke(SaveSlot.SLOT3);
    }
}
