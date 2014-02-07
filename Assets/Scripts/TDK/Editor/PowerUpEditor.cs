/*  This file is part of the "3D Tower Defense Starter Kit" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//custom powerup editor window
public class PowerUpEditor : EditorWindow
{
    [SerializeField]
    PowerUpManager powerUpScript;  //manager reference
    //track if the amount of powerups has been changed while drawing the gui
    bool guiChange = false;

    //top toolbar for displaying powerup types
    int toolbar = 0;
    string[] toolbarStrings = new string[] { "Battle Power Ups", "Passive Power Ups" };

    //inspector scrollbar x/y position, modified by mouse input
    //the battle powerup inspector is divided into two parts, thus two scroll positions
    Vector2 scrollPosBattleSelector;
    Vector2 scrollPosBattleEditor;

    //color to display for each battle powerup type
    Color offensiveColor = new Color(1, 0, 0, 0.4f);
    Color defensiveColor = new Color(0, 0, 1, 0.4f);

    //serialized objects to display,
    //access to the manager script and the selected powerup
    private SerializedObject script;
    private SerializedProperty powerUpToEdit;
    //selected powerup as instance of BattlePowerUp 
    private BattlePowerUp selected;


    //add menu named "PowerUp Settings" to the window menu
    [MenuItem("Window/TD Starter Kit/PowerUp Settings")]
    static void Init()
    {
        //get existing open window or if none, make a new one:
        PowerUpEditor waveEditor = (PowerUpEditor)EditorWindow.GetWindowWithRect(typeof(PowerUpEditor), new Rect(0, 0, 800, 400), false, "PowerUp Settings");
        //automatically repaint whenever the scene has changed (for caution)
        waveEditor.autoRepaintOnSceneChange = true;
    }
   

    //when the window gets opened
    void OnEnable()
    {
        //get reference to PowerUp Manager gameobject if we open the PowerUp Settings
        GameObject pumGO = GameObject.Find("PowerUp Manager");

        //could not get a reference, gameobject not created? debug warning.
        if (pumGO == null)
        {
            Debug.LogError("Current Scene contains no PowerUp Manager.");
            return;
        }

        //get reference to PowerUp Manager script and cache it
        powerUpScript = pumGO.GetComponent<PowerUpManager>();

        //could not get component, not attached? debug warning.
        if (powerUpScript == null)
        {
            Debug.LogWarning("No PowerUp Manager Component found!");
            return;
        }

        //set access to PowerUp Manager as serialized reference
        script = new SerializedObject(powerUpScript);
    }
    

    void OnGUI()
    {
        //we loose the reference on restarting unity and letting the Wave Editor open,
        //or by starting and stopping the runtime, here we make sure to get it again 
        if (powerUpScript == null)
            OnEnable();
        
        //set the targeted script modified by the GUI for handling undo
        Undo.SetSnapshotTarget(powerUpScript, "Changed Settings");
        //save the current state of all objects set with SetSnapshotTarget to internal snapshot
        Undo.CreateSnapshot();

        //display toolbar at the top, followed by a horizontal line
        toolbar = GUILayout.Toolbar(toolbar, toolbarStrings);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        
        //handle toolbar selection
        switch (toolbar)
        {
            //first tab selected
            //draw the battle powerup selector on the left side,
            //and the battle powerup editor on the right side
            case 0:
                EditorGUILayout.BeginHorizontal();
                DrawBattlePowerUpsSelector();
                GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.Width(1));
                DrawBattlePowerUpsEditor();
                EditorGUILayout.EndHorizontal();
                break;
            //second tab selected
            //draw passive powerup window
            case 1:
                DrawPassivePowerUps();
                break;
        }

        //track if the gui has changed by user input
        TrackChange(guiChange);
    }


    //draws a list for all battle powerups
    void DrawBattlePowerUpsSelector()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(270));

        //begin a scrolling view inside GUI, pass in current Vector2 scroll position 
        scrollPosBattleSelector = EditorGUILayout.BeginScrollView(scrollPosBattleSelector, true, true, GUILayout.Height(325), GUILayout.Width(270));

        //iterate over all battle powerups in the main list
        for (int i = 0; i < powerUpScript.battlePowerUps.Count; i++)
        {
            //get the current powerup
            BattlePowerUp powerup = powerUpScript.battlePowerUps[i];

            //differentiate between offensive and defensive powerup,
            //set the gui color correspondingly
            if (powerup is OffensivePowerUp)
                GUI.backgroundColor = offensiveColor;
            else if (powerup is DefensivePowerUp)
                GUI.backgroundColor = defensiveColor;

            //draw a box with the color defined above
            //and reset the color to white
            GUI.Box(new Rect(5, i * 28, 25, 25), i + " ");
            GUI.backgroundColor = Color.white;

            //compare powerup in the list with the currently selected one
            //if it's the selected one, tint the background yellow
            if (powerup == selected)
                GUI.backgroundColor = Color.yellow;
            GUI.Box(new Rect(25, i * 28, 225, 25), "");
            GUI.backgroundColor = Color.white;
        }

        //draw the list of offensive powerups,
        //then draw the list of defensive powerups below
        DrawBattlePowerUps(script.FindProperty("battleOffensive"), powerUpScript.battleOffensive);
        DrawBattlePowerUps(script.FindProperty("battleDefensive"), powerUpScript.battleDefensive);

        //ends the scrollview defined above
        EditorGUILayout.EndScrollView();

        //start button layout at the bottom of the left side
        //draw box with the background color defined at the beginning
        //begin with the offensive powerup add button
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = offensiveColor;
        GUILayout.Box("", GUILayout.Width(20), GUILayout.Height(15));
        GUI.backgroundColor = Color.white;

        //add a new offensive powerup to the list
        if (GUILayout.Button("Add Offensive Power Up"))
        {
            //create new instance
            OffensivePowerUp newOff = new OffensivePowerUp();
            //insert new powerup at the end of the offensive list
            //also add the new powerup to the main list of battle powerups
            powerUpScript.battlePowerUps.Insert(powerUpScript.battleOffensive.Count, newOff);
            powerUpScript.battleOffensive.Add(newOff);
            //mark that the gui changed and update the script values
            guiChange = true;
            script.Update();
            //select the newly created powerup,
            //also select the powerup as active selection for editing
            selected = newOff;
            powerUpToEdit = script.FindProperty("battleOffensive").GetArrayElementAtIndex(powerUpScript.battleOffensive.Count - 1);
        }

        EditorGUILayout.EndHorizontal();
        //continue with the offensive powerup add button
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = defensiveColor;
        GUILayout.Box("", GUILayout.Width(20), GUILayout.Height(15));
        GUI.backgroundColor = Color.white;

        //add a new defensive powerup to the list
        if (GUILayout.Button("Add Defensive Power Up"))
        {
            //create new instance
            DefensivePowerUp newDef = new DefensivePowerUp();
            //add new powerup to the end of the defensive list
            //also add the new powerup to the main list of battle powerups
            powerUpScript.battlePowerUps.Add(newDef);
            powerUpScript.battleDefensive.Add(newDef);
            //mark that the gui changed and update the scipt values
            guiChange = true;
            script.Update();
            //select the newly created powerup as active selection and for editing
            selected = newDef;
            powerUpToEdit = script.FindProperty("battleDefensive").GetArrayElementAtIndex(powerUpScript.battleDefensive.Count - 1);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }


    //draws the "Edit" and "Delete" button for each powerup in the list
    void DrawBattlePowerUps<T>(SerializedProperty list, List<T> repList) where T : BattlePowerUp
    {
        //iterate over the serialized powerup list
        //used for both offensive and defensive powerups
        for (int i = 0; i < list.arraySize; i++)
        {
            //get current actual powerup instance
            BattlePowerUp powerup = repList[i];

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            //draw the powerup name
            GUILayout.Label(powerup.name, GUILayout.Width(145));
            
            //draw edit button that selects this powerup
            if (GUILayout.Button("Edit", GUILayout.Width(35)))
            {
                //select powerup as editable reference
                selected = powerup;
                powerUpToEdit = list.GetArrayElementAtIndex(i);
                //deselect other gui fields,
                //otherwise user input is carried over
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
            }

            //draw delete button for this powerup
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                //unset editable selection
                selected = null;
                powerUpToEdit = null;
                //remove powerup in the main list
                //and actual list for the type
                powerUpScript.battlePowerUps.RemoveAt(i);
                repList.RemoveAt(i);
                //mark that the gui changed and update the scipt values
                guiChange = true;
                script.Update();
                return;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }


    //draws the battle powerup editor
    void DrawBattlePowerUpsEditor()
    {
        //get offensive and defensive powerups and
        //insert them in the main list on every gui update
        //this makes sure that we always display current values
        for (int i = 0; i < powerUpScript.battleOffensive.Count; i++)
            powerUpScript.battlePowerUps[i] = powerUpScript.battleOffensive[i];
        for (int i = 0; i < powerUpScript.battleDefensive.Count; i++)
            powerUpScript.battlePowerUps[i + powerUpScript.battleOffensive.Count] = powerUpScript.battleDefensive[i];
        //update the scipt values
        script.Update();

        //do not draw the following editor window
        //if no editable selection was set
        if (powerUpToEdit == null)
        {
            GUILayout.BeginArea(new Rect((Screen.width / 2) + 50, (Screen.height / 2), 150, 100));
            GUILayout.Label("No Power Up selected.", EditorStyles.boldLabel);
            GUILayout.EndArea();
            return;
        }

        //begin a scrolling view inside GUI, pass in current Vector2 scroll position 
        scrollPosBattleEditor = EditorGUILayout.BeginScrollView(scrollPosBattleEditor);
        //draw custom inspector field for the powerup, including children
        EditorGUILayout.PropertyField(powerUpToEdit, true);
        //ends the scrollview defined above
        EditorGUILayout.EndScrollView();
        //push modified values back to the manager script
        script.ApplyModifiedProperties();
    }


    //draws the passive power up window
    //in development!
    void DrawPassivePowerUps()
    {
        GUILayout.BeginArea(new Rect((Screen.width / 2) - 100, (Screen.height / 2), 250, 100));
        GUILayout.Label("In development.");
        GUILayout.Label("Planned for one of the next releases!");
        GUILayout.EndArea();
    }


    void TrackChange(bool change)
    {
        //if we typed in other values in the editor window,
        //we need to repaint it in order to display the new values
        if (GUI.changed || change)
        {
            //we have to tell Unity that a value of the PowerUp Manager script has changed
            //http://unity3d.com/support/documentation/ScriptReference/EditorUtility.SetDirty.html
            EditorUtility.SetDirty(powerUpScript);
            //Register the snapshot state made with CreateSnapshot so the user can later undo back to that state
            Undo.RegisterSnapshot();
            //repaint editor GUI window
            Repaint();
        }
        else
            //clear the snapshot at end of call
            Undo.ClearSnapshotTarget();
    }
}