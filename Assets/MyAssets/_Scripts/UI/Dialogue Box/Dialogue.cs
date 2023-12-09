using UnityEngine;
using UnityEngine.Events;

namespace ColdClimb.UI{
    [System.Serializable]
    public class Dialogue{
        [TextArea(3, 3)]
        public string[] sentences;
        public bool isQuestion = false;
        [TextArea(2, 2)]
        public string answerOneText;
        [TextArea(2, 2)]
        public string answerTwoText;

        public UnityEvent answerOneEvent;
        public UnityEvent answerTwoEvent;
        public UnityEvent eventAfterDialogueEnds;
    }
}
