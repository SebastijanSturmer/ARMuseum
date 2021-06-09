using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    
    public void OnLoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnLoadQuiz()
    {
        SceneManager.LoadScene("Quiz");
    }
    public void OnLoadAR()
    {
        SceneManager.LoadScene("AR");
    }
}
