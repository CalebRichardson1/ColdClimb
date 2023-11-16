using System;
using UnityEngine;

/// <summary>
/// Scriptable object that holds data based on the strucs defined. DataHolder is the base from all other dataholder inherit from.
/// </summary>
namespace ColdClimb.Global.SaveSystem{
    public abstract class DataHolder : ScriptableObject{
        public abstract event Action LoadValuesCallback;

        public virtual void Intialize(){

        }

        protected virtual void OnSave(){

        }
        protected virtual void OnLoad(){

        }
        protected virtual void OnNewGame(){

        }
    }
}

