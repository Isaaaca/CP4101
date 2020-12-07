using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        public string speaker = "";
        public string lookAt = "";
        [TextArea()]
        public string text = "";
        public DialogueChoice[] options;

    }

    [Serializable]
    public class DialogueChoice
    {
        public string text = "";
        public Dialogue nextDialogue = null;
    }

    public DialogueLine[] dialogueLines;
    //for cases where branching dialogue merges
    public Dialogue nextDialogue = null;
}
