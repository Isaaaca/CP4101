using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int tutorialSceneIndex = 1;
    [SerializeField] private Button continueButton = null;
    [SerializeField] private Button newGameButton = null;
    [SerializeField] private TMPro.TextMeshProUGUI participantIDText = null;
    [SerializeField] private ScreenFader sceneChangeScreen = null;

    private static bool loaded = false;

    private void Awake()
    {
        sceneChangeScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        BgmManager.instance.FadeOut(0f);

        if (!loaded)
        {
            SaveManager.LoadFromPrefs();
            SaveManager.SessionStartTime = System.DateTime.Now.ToString("dd/mm/yyyy HH:mm:ss");

            loaded = true;
        }

        if (SaveManager.participantID == "")
        {
            continueButton.gameObject.SetActive(false);
            participantIDText.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
        }
        else
        {
            participantIDText.gameObject.SetActive(true);
            participantIDText.text += SaveManager.participantID;
            continueButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }

    }
    public void LoadLevel(int sceneIndex)
    {
        SaveManager.playerSpawnPoint = Vector2.zero;
        SaveManager.playerLust = 0;
        SaveManager.ClearLevelData("0"+(sceneIndex - 1).ToString());
        StartCoroutine(TransitionToScene(sceneIndex));

    }
    IEnumerator TransitionToScene(int levelIndex)
    {
        sceneChangeScreen.FadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelIndex);

    }

    public void NewGame()
    {
        SaveManager.Clear();
        SaveManager.SaveToPrefs();
        StartCoroutine(TransitionToScene(tutorialSceneIndex));
    }


    public void ContinueGame()
    {
        StartCoroutine(TransitionToScene(SaveManager.currentLevelSceneCode));
        
    }
}
