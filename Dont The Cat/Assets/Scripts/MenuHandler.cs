using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioHandler.Instance.PlayLoopSound(Clip.Meow_Melody);
    }

    public void onPlayButtonClick()
    {
        AudioHandler.Instance.StopLoopSound();

        AudioHandler.Instance.PlaySingleSound(Clip.Button_Press);

        SceneManager.LoadScene("Level Design");
    }
}
