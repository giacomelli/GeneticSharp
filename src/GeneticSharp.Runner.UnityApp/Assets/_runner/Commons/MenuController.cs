using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    private Canvas m_canvas;
    private GameObject m_samples;
    public Text Logo;

	private void Start()
	{
        m_canvas = GetComponent<Canvas>();
        m_samples = transform.Find("Panel/Samples").gameObject;
	}

	private void Update()
	{
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            m_samples.SetActive(!m_samples.activeSelf);
        }
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
