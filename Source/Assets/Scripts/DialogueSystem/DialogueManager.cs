using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static event Action<bool> OnDialogueStartEnd = (active) => { };
    public static event Action<string> OnDialogueEvent = (dialogueEventID) => { };

    [SerializeField] private CamController camController = null;
    [SerializeField] private TextMeshProUGUI nameText = null;
    [SerializeField] private TextMeshProUGUI mainText = null;
    [SerializeField] private TextMeshProUGUI[] optionTexts = null;
    [SerializeField] private Image[] selectionImages = null;
    [SerializeField] private GameObject dialogueBox = null;
    [SerializeField] private AudioClip nextAudioClip = null;
    [SerializeField] private AudioClip changeSelectionAudioClip = null;

    private Dialogue dialogue;
    private int currLineIndex = 0;
    private int currSelection = 0;
    private int numOpt = 0;
    private GameObject player;
    private Dictionary<string,Transform> cameraTargets = new Dictionary<string, Transform>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.GetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueBox.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space)) NextLine();
            if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeSelection(true);
            if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeSelection(false);
        }
    }

    public void LoadDialogue(Dialogue dialogue)
    {
        camController.SwitchCamera(CamController.CameraMode.Center);
        this.dialogue = dialogue;
        GetSpeakers(dialogue);
        currLineIndex = 0;
        UpdateDialogueDisplay();
        dialogueBox.SetActive(true);
        OnDialogueStartEnd(true);
    }

    private void GetSpeakers(Dialogue dialogue)
    {
        cameraTargets.Clear();
        foreach (Dialogue.DialogueLine  line in dialogue.dialogueLines)
        {
            string target = line.lookAt;
            if (target == "")
                target = line.speaker;

            if (target!="" && !cameraTargets.ContainsKey(target))
            {
                var gameObj = GameObject.Find(target);
                if(gameObj != null)
                {
                    cameraTargets.Add(target, gameObj.transform);
                }
            }
        }
    }

    public void NextLine()
    {
        var currentLine = dialogue.dialogueLines[currLineIndex];
       
        currLineIndex++;
        if (currLineIndex >= dialogue.dialogueLines.Length)
        {
            if (dialogue.nextDialogue != null)
            {
                LoadDialogue(dialogue.nextDialogue);
            }
            else if (currentLine.options.Length > 0 &&
                    currentLine.options[currSelection - 1].nextDialogue != null)
            {
                OnDialogueEvent(dialogue.name + (char)(currSelection+64));
                LoadDialogue(currentLine.options[currSelection - 1].nextDialogue);
            }
            else
            {
                //end convo
                if (nextAudioClip != null)
                {
                    AudioManager.PlayClip(nextAudioClip);
                }
                dialogueBox.SetActive(false);
                //since dialogues are contained within sequences, only choices need to be logged.
                //OnDialogueEvent(dialogue.name);
                OnDialogueStartEnd(false);
            }

        }
        else
        {
            UpdateDialogueDisplay();
        }
    }

    public void CloseDialogue()
    {
        dialogueBox.SetActive(false);
    }

    private void ChangeSelection(bool increment)
    {
        
        if (increment && currSelection < numOpt)
        {
            currSelection++;
            UpdateSelectionUI();
            if (changeSelectionAudioClip != null)
            {
                AudioManager.PlayClip(changeSelectionAudioClip);
            }
        }
        else if(!increment && currSelection > 1)
        {
            currSelection--;
            UpdateSelectionUI();
            if (changeSelectionAudioClip != null)
        {
            AudioManager.PlayClip(changeSelectionAudioClip);
        }
        }
    }

    private void UpdateSelectionUI()
    {
        
        for (int i = 0; i < selectionImages.Length; i++)
        {
            if (i == currSelection-1) selectionImages[i].enabled = true;
            else selectionImages[i].enabled = false;
        }
    }

    private void UpdateDialogueDisplay()
    {

        if (nextAudioClip != null)
        {
            AudioManager.PlayClip(nextAudioClip);
        }
        string target = dialogue.dialogueLines[currLineIndex].lookAt;
        if (target == "")
            target = dialogue.dialogueLines[currLineIndex].speaker;

        if (target!="" && cameraTargets.ContainsKey(target))
        {
            camController.ChangeFollowTarget(cameraTargets[target]);
            if (cameraTargets[target] != player.transform)
            {
                CharacterMovementController pController = player.GetComponent<CharacterMovementController>();
                if (pController!=null && !pController.IsFacing(cameraTargets[target].position))
                {
                    pController.Turn();
                }
            }
        }

        nameText.text = dialogue.dialogueLines[currLineIndex].speaker;
        mainText.text = dialogue.dialogueLines[currLineIndex].text;
        numOpt = dialogue.dialogueLines[currLineIndex].options.Length;
        if (numOpt > 0)
        {
            for (int i = 0; i < numOpt; i++)
            {
                optionTexts[i].text = dialogue.dialogueLines[currLineIndex].options[i].text;
            }
            currSelection = 1;

        }
        else currSelection = 0;
        for (int i = numOpt; i < optionTexts.Length; i++)
        {
            optionTexts[i].text = "";
        }
        UpdateSelectionUI();
    }
}
