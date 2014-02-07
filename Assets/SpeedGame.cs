using UnityEngine;
using System.Collections;

public class SpeedGame : MonoBehaviour
{
    public UISprite Icon;
    public GameObject PauseB;
    public GameObject PlayB;
    public Color Color;

    public void Pause()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            PauseB.SetActive(false);
            PlayB.SetActive(true);
            Icon.color = Color.white;
        }
        else
        {
            Time.timeScale = 1;
            PauseB.SetActive(true);
            PlayB.SetActive(false);
            Icon.color = Color.white;
        }
        
    }
    public void Speed()
    {
        if (Time.timeScale > 1)
        {
            Time.timeScale = 1;
            Icon.color = Color.white;

        }
        else
        {
            Time.timeScale = 1.5f;
            Icon.color = Color;
            PauseB.SetActive(true);
            PlayB.SetActive(false);
        }
    }
}
