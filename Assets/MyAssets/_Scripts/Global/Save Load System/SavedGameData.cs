namespace ColdClimb.Global.SaveSystem{
    /// <summary>
    /// A class that holds all the data relating to the game.
    /// </summary>
    [System.Serializable]
    public class SavedGameData{
        public ContinueGame CurrentContinueGame = new ContinueGame();
    }

    [System.Serializable]
    public struct ContinueGame{
        public SaveSlot previousSlot;
        public string previousSaveFileName;
    }
}

