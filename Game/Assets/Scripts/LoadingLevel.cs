using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class LoadingLevel : MonoBehaviour
{
    public Slider slider;
    public GameObject progress_message;
    // Start is called before the first frame update
    public void LoadLevel ()
    {
        StartCoroutine(LoadAsync());
        
    }

    IEnumerator LoadAsync ()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("SampleScene");


        while(!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            slider.value = progress;
            progress_message.GetComponent<TextMeshPro>().SetText(((int)(progress * 100)).ToString() + "%");
            yield return null;
        }
    }
}
