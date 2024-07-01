using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public RawImage pine;
    public RawImage gum;

    public void OnStartClick() {
        if (pine.enabled) {
            PlayerPrefs.SetString("Species Name", "Pine");
            SceneManager.LoadScene("Pine"); }
        else { 
            PlayerPrefs.SetString("Species Name", "Gum");
            SceneManager.LoadScene("Gum"); }
    }

    public void OnNextTreeClick() {
        pine.enabled = false;
        gum.enabled = true;
    }

    public void OnPreviousTreeClick() {
        gum.enabled = false;
        pine.enabled = true;
    }

    public void OnCreditsClick() {
        SceneManager.LoadScene("Credits");
    }

    public void OnExitClick() {
        Application.Quit();
    }

    public void OnBackClick() {
        SceneManager.LoadScene("MainMenu");
    }

}
