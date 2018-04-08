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
        Logo.enabled = false;
        SceneManager.LoadScene($"{sceneName}Scene");
    }
}
