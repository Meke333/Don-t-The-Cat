using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AudioHandler.Instance.PlaySound(Clip.Cricket, true);
    }

    public void onPlayButtonClick()
    {
        AudioHandler.Instance.StopSound();
        SceneManager.LoadScene("DATLEVEL");
    }
}
