using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayLvlOne()
    {
        //Scenes can be loaded by build index or by scene name eg. "LevelOne"
        SceneManager.LoadScene("LevelOne");
        
    }

        public void PlayLvlTwo()
    {
        SceneManager.LoadScene("LevelTwo");
        
    }

    public void PlayLvlThree()
    {
        SceneManager.LoadScene("LevelThree");
        
    }

    public void QuitGame()
    {
        //Quit game code cant be tested unitl game is built so we use debug.log
        Debug.Log("Quit game");
        Application.Quit();
    }
}
