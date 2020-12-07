using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public string levelCode = "";
    [SerializeField] PlayerController _player = null;
    [SerializeField] ScreenFader screen = null;
    [SerializeField] CamController camController = null;
    [SerializeField] CutsceneDirector cutsceneDirector = null;
    [SerializeField] GameMenu levelEndMenu = null;
    [SerializeField] string lastCondition = "";
    [SerializeField] GameEventDictionary eventDictionary = null;

    public static event Action<bool> SetGameplayEnabled = (enable) => { };


    private bool gameEventRunning = false;
    private bool isReloading = false;
    private string currRoom = "";

    private static PlayerController player;
    public static GameObject GetPlayer()
    {
        return player.gameObject;
    }

    private void Awake()
    {
        player = _player;
        Interactable.OnInteractEvent += HandleInteractEvent;
        Room.OnEnterRoom += HandleEnterRoomEvent;
        Character.OnCharacterDeath += HandleCharacterDeath;
        PlayerController.OnGameOver += HandleGameOver;
        CutsceneDirector.OnSequenceEnd += HandleSequenceEnd;
        DialogueManager.OnDialogueEvent += HandleDialogueEvent;
        AreaTrigger.OnEnterAreaTrigger += HandleTrigger;
    }

    private void OnDestroy()
    {
        Interactable.OnInteractEvent -= HandleInteractEvent;
        Room.OnEnterRoom -= HandleEnterRoomEvent;
        Character.OnCharacterDeath -= HandleCharacterDeath;
        PlayerController.OnGameOver -= HandleGameOver;
        CutsceneDirector.OnSequenceEnd -= HandleSequenceEnd;
        DialogueManager.OnDialogueEvent -= HandleDialogueEvent;
        AreaTrigger.OnEnterAreaTrigger -= HandleTrigger;
    }

    private void HandleTrigger(string triggerCode)
    {
        CheckForScriptedEvent(triggerCode);
    }

    private void HandleDialogueEvent(string eventCode)
    {
        SaveManager.AddEvent(eventCode);
        CheckForScriptedEvent(eventCode);
    }

    private void HandleSequenceEnd(Sequence sequence)
    {
        //TODO: check for continued event/conditional dialogue
        SaveManager.AddEvent(sequence.name);
        screen.FadeIn();
        gameEventRunning = false;
        camController.SwitchCamera(CamController.CameraMode.Follow);
        SetGameplayEnabled(true);
        CheckForScriptedEvent(sequence.name);
    }

    private void HandleGameOver(char type)
    {
        if (!isReloading)
        {
            SetGameplayEnabled(false);
            string code = levelCode + type + currRoom;
            SaveManager.IncrementCounter(code);
            cutsceneDirector.CutSequence();
            camController.SwitchCamera(CamController.CameraMode.Follow);
            screen.CustomFade(1f, 2f);
            if (type == 'L')
            {
                //increase sensitivity
            }
            isReloading = true;
            CheckForScriptedEvent(code);
        }
    }

    private void HandleCharacterDeath(Character character)
    {
        if (character.name == "Boss")
        {
            string code = levelCode + character.name;
            SaveManager.AddEvent(code);
            CheckForScriptedEvent(code);
        }
        else if (character.name == "Lunarian")
        {
            SetGameplayEnabled(false);
            string code = levelCode + character.name.Substring(0, 3) + currRoom;
            SaveManager.IncrementCounter(code);
            cutsceneDirector.CutSequence();
            camController.SwitchCamera(CamController.CameraMode.Follow);
            screen.CustomFade(1f, 2f);
            isReloading = true;
        }
        else
        {
            SaveManager.IncrementCounter(levelCode+"M" +currRoom+ character.name.Substring(0, 3));
        }
    }

    private void HandleEnterRoomEvent(Room room)
    {
        currRoom = room.name;
        CheckForScriptedEvent(room.name);
    }

    private void HandleInteractEvent(string id, Interactable interactable)
    {
        Sequence seq = interactable.GetSequence();
        if (seq != null) StartSequence(seq);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading &&! gameEventRunning)
        {
            if (!screen.isTransitioning())
            {
                if (screen.getOpacity() == 1)
                {
                    player.Respawn();
                    Meter playerLust = player.GetLust();
                    if (playerLust.Get() == playerLust.GetMax())
                    {
                        playerLust.Set(0);
                    }

                    SaveManager.playerLust = player.GetLust().Get();
                    SaveManager.SaveToPrefs();
                    screen.FadeIn();
                }
                else if (screen.getOpacity() == 0)
                {
                    SetGameplayEnabled(true);
                    player.PlayWakeAnim();
                    isReloading = false;
                }
            }
        }
    }

    private void CheckForScriptedEvent(string key)
    {
        if (eventDictionary.ContainsKey(key))
        {
            Sequence seq = eventDictionary[key].GetSequence();
            if (seq != null)
            {
                StartSequence(seq);
                gameEventRunning = true;
            }
        }
        else if (key == lastCondition)
        {
            isReloading = false;
            screen.FadeToBlack();
            player.enabled = false;
            //Load Level End screen
            levelEndMenu.ShowLevelEndMenu();
        }
    }


    private void StartSequence(Sequence sequence)
    {
        SetGameplayEnabled(false);
        cutsceneDirector.PlaySequence(sequence);

    }

    public void SetGamePause(bool paused)
    {
        Time.timeScale = paused ? 0 : 1;
        Input.ResetInputAxes();
        if (!gameEventRunning)
            SetGameplayEnabled(!paused);
    }
}
