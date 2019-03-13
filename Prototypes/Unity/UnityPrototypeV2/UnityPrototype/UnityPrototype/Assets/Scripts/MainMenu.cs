using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button btn_start;
    public Button btn_options;
    public Button btn_quit;

    // Start is called before the first frame update
    void Start()
    {
        btn_quit.onClick.AddListener(ButtonQuit);
        btn_start.onClick.AddListener(ButtonStart);
        btn_options.onClick.AddListener(ButtonOptions);
    }

    void ButtonStart()
    {
        Debug.Log("loading samplescene");
        SceneManager.LoadScene("SampleScene");
    }

    void ButtonOptions()
    {
        // open options ui
    }
        
    void ButtonQuit()
    {
        Application.Quit();
    }
}
