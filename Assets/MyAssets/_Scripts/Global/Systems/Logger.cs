using UnityEngine;

[CreateAssetMenu(menuName = "Systems/Logger", fileName ="NewLogger")]
public class Logger : ScriptableObject
{
    [SerializeField] private bool logMessages;

    public void Log(string message, Object sender){
        if(logMessages) Debug.Log(message, sender);
    }
    public void Log(string message){
        if(logMessages) Debug.Log(message);
    }
}
