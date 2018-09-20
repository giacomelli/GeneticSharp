using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    private GameObject m_samples;
    private string m_selectedSceneName;
    public Image Logo;
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
        Logo.enabled = false;
        m_selectedSceneName = sceneName;

        if (Advertisement.IsReady() && !sceneName.Equals("About"))
        {
            var options = new ShowOptions { resultCallback = OpenScene };
            Advertisement.Show(options);
        }
        else
        {
            OpenScene(ShowResult.Skipped);
        }
    }

    void OpenScene(ShowResult result)
    {
        CurrentInfo.transform.position = new Vector3(CurrentInfo.transform.position.x, 60, 0);
        PreviousInfo.transform.position = new Vector3(PreviousInfo.transform.position.x, 60, 0);
       
        SceneManager.LoadScene($"{m_selectedSceneName}Scene");
    }

    public void OpenLink()
    {
        Application.OpenURL("https://github.com/giacomelli/GeneticSharp");
    }
}