using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioHandler.Instance.PlayLoopSound(Clip.Cricket);
    }

    public void onPlayButtonClick()
    {
        AudioHandler.Instance.PlaySingleSound(Clip.Bounce);

        AudioHandler.Instance.StopLoopSound();

        SceneManager.LoadScene("Level Design");
    }
}
