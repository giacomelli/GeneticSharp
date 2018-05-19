using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    private GameObject m_samples;
    public Text Logo;
    public RectTransform CurrentInfo;
    public RectTransform PreviousInfo;

	private void Start()
	{
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
        CurrentInfo.transform.position = new Vector3(CurrentInfo.transform.position.x, 60, 0);
        PreviousInfo.transform.position = new Vector3(PreviousInfo.transform.position.x, 60, 0);

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
