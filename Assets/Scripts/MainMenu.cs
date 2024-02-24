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

        public void PlayTestLvl()
    {
        //Scenes can be loaded by build index or by scene name eg. "LevelOne"
        SceneManager.LoadScene("TestLevel");
        
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
