using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    public string nextScene;
    private bool notTriggered = true;

    private void OnTriggerEnter(Collider other)
    {
        if (notTriggered && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(LoadScene());
            notTriggered = false;
        }
    }

    IEnumerator LoadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
