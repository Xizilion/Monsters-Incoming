using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour
{
    public string SceneToLoad;

    public void LoadScene()
    {
        if (Loader.Level == 0)
            Application.LoadLevel(SceneToLoad);
        else
        {
            Application.LoadLevel(Loader.Level);
        }
    }
}
