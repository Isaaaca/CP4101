using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private ScreenFader screen = null;
    [SerializeField] private ScreenFader sceneChangeScreen = null;
    [SerializeField] private GameObject levelEndPanel = null;
    [SerializeField] private GameObject levelEndDefaultSelection = null;
    [SerializeField] private GameObject pausePanel = null;
    [SerializeField] private GameObject pauseDefaultSelection = null;
    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private LevelFeedback[] feedbacks = null;
    [SerializeField] private Slider sfxVolumeSlider = null;
    [SerializeField] private Slider bgmVolumeSlider = null;

    private void Awake()
    {
        sceneChangeScreen.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(levelEndPanel.activeSelf == false
            && Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }
    public void ShowLevelEndMenu()
    {
        levelEndPanel.SetActive(true);
        foreach( LevelFeedback fb in feedbacks)
        {
            fb.LoadFeedback();
        }
        screen.FadeIn();
        
        SaveManager.currentLevelSceneCode = SceneManager.GetActiveScene().buildIndex + 1;
        SaveManager.playerSpawnPoint = Vector2.zero;
        SaveManager.SaveToPrefs();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(levelEndDefaultSelection);

        StartCoroutine(SaveManager.PostLevelData(gameManager.levelCode));

    }

    public void TogglePause()
    {
        if (pausePanel.activeSelf) Unpause();
        else Pause();
    }
    public void Pause()
    {
        gameManager.SetGamePause(true);
        pausePanel.SetActive(true);
        bgmVolumeSlider.value = BgmManager.instance.GetVolume();
        sfxVolumeSlider.value = AudioManager.instance.GetVolume();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseDefaultSelection);
    }

    public void ChangeSfxVolume(float volume)
    {
        AudioManager.instance.SetVolume(volume);
    }
    public void ChangeBgmVolume(float volume)
    {
        BgmManager.instance.SetVolume(volume);
    }

    public void Unpause()
    {
        gameManager.SetGamePause(false);
        EventSystem.current.SetSelectedGameObject(null);
        pausePanel.SetActive(false);
    }

    public void NextLevel()
    {
        int nexwLvlSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        SaveManager.playerSpawnPoint = Vector2.zero;
        SaveManager.playerLust = 0;
        SaveManager.ClearLevelData("0" + (nexwLvlSceneIndex - 1).ToString());
        StartCoroutine(TransitionToScene(nexwLvlSceneIndex));
    } 

    IEnumerator TransitionToScene(int levelIndex)
    {
        sceneChangeScreen.FadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelIndex);

    }

    
    public void MainMenu()
    {
        if (pausePanel.activeSelf) gameManager.SetGamePause(false);
        StartCoroutine(TransitionToScene(0));
    }
}
