using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas optionCanvas;
    public void ReloadScene() {
        SceneManager.LoadScene("Game");
    }
    public void Quit() {
        Application.Quit();
    }

    public void SelectCarScene() {
        SceneManager.LoadScene("SelectCar");
    }
    public void ShowOptions() {
        mainCanvas.enabled = false;
        optionCanvas.enabled = true;
    }
}
