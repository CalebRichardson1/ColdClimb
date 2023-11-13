using UnityEngine;

namespace ColdClimb.Global.SaveSystem{
    /// <summary>
    /// A class that holds all the structs relating to our options.
    /// </summary> 

    [System.Serializable]
    public class SavedOptions{
        public Options CurrentSettings = new Options();
    }

    [System.Serializable]
    public struct Options{
        [Header("Audio")]
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;

        [Header("Video")]
        public VideoResolution resolution;
        public bool isFullScreen;
    }

    [System.Serializable]
    public struct VideoResolution{
        public VideoResolution(int hor, int vert){
            horizontal = hor;
            vertical = vert;
        }    
        
        public void SetValues(int hor, int vert){
            horizontal = hor;
            vertical = vert;
        }

        public int horizontal, vertical; 
    }
}


