using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    //public Camera menuCamera;
    public Canvas canvas;
    //public Button arcadeButton, survivalButton, settingsButton, aboutButton, quitButton;

    public void startArcadeGame()
    {
        //call game controller with params
    }

    public void startSurvivalGame()
    {
        //call game controller with params
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    
    /// <summary>
    /// Quits the application
    /// </summary>
    public void quitGame()
    {
        AndroidLikeDialog.instance.show("Are you sure?", "Are you sure you want to quit?", () => { Application.Quit(); });
    }
}
