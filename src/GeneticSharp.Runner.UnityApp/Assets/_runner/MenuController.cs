using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public void OpenTsp()
    {
        SceneManager.LoadScene("TspScene");
    }

    public void Open(string sceneName)
    {
        SceneManager.LoadScene($"{sceneName}Scene");
    }
}
