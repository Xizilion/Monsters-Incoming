/*  This file is part of the "3D Tower Defense Starter Kit" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using System.Collections;

//Simple GUI script that maps buttons to load a scene
public class Intro : MonoBehaviour
{
    //list of buttons/scene names
    public GUIMenuItem[] items;

    void Start()
    {
        //assign each button to the method LoadScene()
        for (int i = 0; i < items.Length; i++)
            UIEventListener.Get(items[i].button).onClick += LoadScene;
    }


    //if a button gets clicked,
    //iterate over all maps until we find the corresponding button,
    //then load the scene assigned to the button
    void LoadScene(GameObject button)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (button == items[i].button)
                Application.LoadLevel(items[i].sceneToLoad);
        }
    }


    //class that stores a button object 
    //and a string for the scene name
    [System.Serializable]
    public class GUIMenuItem
    {
        public GameObject button;
        public string sceneToLoad;
    }
}
