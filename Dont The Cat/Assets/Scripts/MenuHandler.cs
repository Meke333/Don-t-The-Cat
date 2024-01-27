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

    // Update is called once per frame
    void Update()
    {

    }

    public void onPlayButtonClick()
    {
        AudioHandler.Instance.StopLoopSound();

        AudioHandler.Instance.PlaySingleSound(Clip.Bounce);
        SceneManager.LoadScene("DATLEVEL");
    }
}
