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
    public void CrosswordLevel1()
    {
        SceneManager.LoadScene("Crossword_Level1");
    }

    public void CrosswordLevel2()
    {
        SceneManager.LoadScene("Crossword_Level2");
    }
    public void CrosswordLevel3()
    {
        SceneManager.LoadScene("Crossword_Level3");
    }

    public void Archery()
    {
        SceneManager.LoadScene("Archery");
    }

    public void Archery1()
    {
        SceneManager.LoadScene("Archery1");
    }
}
