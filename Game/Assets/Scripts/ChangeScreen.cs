using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public GameObject progress_message;
    // Start is called before the first frame update
    public void PlayGame()
    {
        StartCoroutine(LoadAsync());

    }

    IEnumerator LoadAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("SampleScene");


        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            slider.value = progress;
            //progress_message.GetComponent<TextMeshProUGUI>().SetText(((int)(progress * 100)).ToString() + "%");
            yield return null;
        }
    }




    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
