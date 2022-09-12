using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
	public void ReloadScene()
    {
    	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Home()
    {
        SceneManager.LoadScene("Home");
    }
    public void Crossword()
    {
        SceneManager.LoadScene("Crossword");
    }

    public void Archery()
    {
        SceneManager.LoadScene("Archery");
    }
}
