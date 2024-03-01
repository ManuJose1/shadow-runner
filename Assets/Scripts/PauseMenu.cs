using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(GameIsPaused)
            {
                Resume();
                Cursor.lockState = CursorLockMode.Confined;
            }else
            {
                Pause();
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);        
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitToMain()
    {
        Debug.Log("Quitting to Main Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
