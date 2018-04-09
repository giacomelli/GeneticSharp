using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public Text Logo;

	private void Start()
	{
        DontDestroyOnLoad(this);
	}

	public void Open(string sceneName)
    {
        Logo.text = "Loading...";
        Logo.enabled = true;
        SceneManager.sceneLoaded += delegate
        {
            Logo.enabled = false;
        };
        SceneManager.LoadScene($"{sceneName}Scene");
    }

    public void OpenLink()
    {
        Application.OpenURL("https://github.com/giacomelli/GeneticSharp");
    }
}
