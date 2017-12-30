using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	public void PlayTraining()
	{
		SceneManager.LoadScene (1);
	}

	public void PlayAutonomous() 
	{
		SceneManager.LoadScene (2);
	}

    public void Exit()
    {
        Application.Quit();
    }
}
