using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] private ScreenFader sceneChangeScreen = null;
    [SerializeField] private GameObject defaultSelection = null;

    private void Awake()
    {
        sceneChangeScreen.gameObject.SetActive(true);
    }
    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelection);
        
    }
    IEnumerator TransitionToScene(int levelIndex)
    {
        sceneChangeScreen.FadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelIndex);

    }


    public void MainMenu()
    {
        StartCoroutine(TransitionToScene(0));
    }

    public void GoToSurvey()
    {
        Application.OpenURL("https://nus.syd1.qualtrics.com/jfe/form/SV_1C7YYjMGQksLIuV?ParticipantID=" + SaveManager.participantID);
    }
}
