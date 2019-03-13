using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    Button btn_restart;

    // Start is called before the first frame update
    void Start()
    {
        btn_restart = GetComponent<Button>();
        btn_restart.onClick.AddListener(Restart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}
