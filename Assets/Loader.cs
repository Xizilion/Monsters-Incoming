using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public static Loader Instance;
    public static int Level;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        Level = Application.loadedLevel;

        if (Level == 4)
            Level = 0;
    }
}
