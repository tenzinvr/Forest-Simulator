using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public GameObject saturated;
    public TimeManager timeManager;
    
    public void OnNextClick() {
        timeManager.Tick();
    }

    public void OnRestartClick() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClick() {
        SceneManager.LoadScene("MainMenu");
    }

/// <summary>
/// The forest is saturated when it has reached a point of equilibrium, when few positions are left
/// </summary>
    public void Saturated() {
        saturated.SetActive(true);
    }
    
}
 