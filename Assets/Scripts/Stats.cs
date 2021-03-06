﻿using UnityEngine;
using System.Collections;

// Assessor (i.e. Unity's PlayerPrefs)
// For a step-based tutoring system,
// the assessor needs to receive all the steps
// that the student did, and for each step,
// the step analyzer needs to send the assessor:
// 1. A step history e.g., whether the student got the step right
//    on the first attempt, asked for a hint, gave up, etc.
// 2. The knowledge components exercised by the step
// The assessor maintains the learner model,
// which has a database of all knowledge components
// and perhaps much other information about the student/learner.
// Each knowledge component has a number,
// category or some other value that represents the learner's
// current competence on that knowledge component.
// The assessor's job is to update the measures of competence
// based on the input it receives from the step analyzer.


public class Stats : MonoBehaviour
{
    public GUISkin skin;
    public GUISkin btnSkin;
    private string[] history;

	// Use this for initialization
	void Start ()
    {
        history = PlayerPrefs.GetString("StepHistory").Split('\t');
        /*for (int i = 0; i < history.Length - 1; i++)
        {
            int numCorrect = PlayerPrefs.GetInt("c" + i);
            int numIncorrect = PlayerPrefs.GetInt("i" + i);
            Debug.Log(numCorrect + ", " + numIncorrect);
            // pie chart set data?
        }//*/
	}
	
	// Update is called once per frame
	void Update () {
	}
    
    void OnGUI()
    {
        GUI.skin = skin;

        // TODO: GUI.BeginScrollView() if we ever decide to add more than four tasks
        // most likely by creating TaskSelector.cs which imports tasks from a text file
        // and chooses tasks accordingly to student's current level of knowledge
        // when asked for by the pedagogical model during gameplay...

        drawTable();

        // feedback shown in a GUI.Window
        GUI.skin = skin;
        if (showFeedback)
        {
            windowRect = GUI.Window(0, windowRect, windowFunc, windowTitle);
        }

        // Possible Save button?

        // Restart button
        GUI.skin = btnSkin;
        if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.8f, Screen.width * 0.25f, Screen.height * 0.1f), "Restart"))
        {
            Application.LoadLevel("Title");
        }
    }

    // custom draw table function I made since unity doesn't have a GUI.Table >.>
    void drawTable()
    {
        // draw background
        Rect bgRect = new Rect(0, Screen.height * 0.1f, Screen.width, Screen.height * 0.4f);
        GUI.backgroundColor = Color.black;
        GUI.Box(bgRect, "");

        // Start the drawing area with the Table's title
        GUILayout.BeginArea(bgRect);
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.skin.label.wordWrap = false;
        GUILayout.Label("Performance");

        // Draw the table by drawing one column at a time
        GUILayout.BeginHorizontal();

        GUI.skin.label.normal.textColor = Color.white;
        
        // Column 1: Task number 
        GUILayout.BeginVertical();
        GUILayout.Label("\nTask #");
        for (int i = 0; i < history.Length - 1; i++)
        {
            GUILayout.Label(""+(i + 1) + ":");
        }
        GUILayout.EndVertical();

        // buffer
        GUILayout.Label("\t");

        // Column 2: Specified Task
        GUILayout.BeginVertical();
        GUILayout.Label("\nQuestion");
        for (int i = 0; i < history.Length - 1; i++)
        {
            GUILayout.Label(history[i].Split('\n')[0]);
        }
        GUILayout.EndVertical();

        // buffer
        GUILayout.Label("\t");
        
        GUI.skin.label.normal.textColor = Color.green;
        
        // Column 3: Number of correct
        GUILayout.BeginVertical();
        GUILayout.Label("Number of\nCorrect:");
        for (int i = 0; i < history.Length - 1; i++)
        {
            GUILayout.Label("" + PlayerPrefs.GetInt("c" + i));
        }
        GUILayout.EndVertical();

        // buffer
        GUILayout.Label("\t");

        GUI.skin.label.normal.textColor = Color.red;
        
        // Column 4: Number of incorrect
        GUILayout.BeginVertical();
        GUILayout.Label("Number of\nWrong:");
        for (int i = 0; i < history.Length - 1; i++)
        {
            GUILayout.Label("" + PlayerPrefs.GetInt("i" + i));
        }
        GUILayout.EndVertical();

        // buffer
        GUILayout.Label("\t");

        GUI.skin.label.normal.textColor = Color.green;

        // Column 5: Total Attempts
        GUILayout.BeginVertical();
        GUILayout.Label("Total\nAttempts:");
        for (int i = 0; i < history.Length - 1; i++)
        {
            GUILayout.Label("" + (PlayerPrefs.GetInt("c" + i) + PlayerPrefs.GetInt("i" + i)));
        }
        GUILayout.EndVertical();

        // buffer
        GUILayout.Label("\t");

        // Column 6: Percentile: C/(C+I) * 100%
        GUILayout.BeginVertical();
        GUILayout.Label("\nPercentage:");
        for (int i = 0; i < history.Length - 1; i++)
        {
            int numCorrect = PlayerPrefs.GetInt("c" + i);
            int numIncorrect = PlayerPrefs.GetInt("i" + i);
            GUILayout.Label("" + (((float)numCorrect / (float)(numCorrect + numIncorrect)) * 100) + "%");
        }
        GUILayout.EndVertical();

        // buffer
        GUILayout.Label("\t");

        // I do this because the labels are bigger than the buttons
        GUI.backgroundColor = Color.white; // buttons should be white, not black like the background
        float height = GUI.skin.label.CalcHeight(new GUIContent("Hello World"), 100.0f);

        // Column 6: Buttons
        GUILayout.BeginVertical();
        GUILayout.Label("\nSteps");
        GUI.skin = btnSkin;
        GUI.skin.button.fixedHeight = height;
        GUI.skin.button.fontSize = 13;
        for (int i = 0; i < history.Length - 1; i++)
        {
            if (GUILayout.Button("Show Steps"))
            {
                if (showFeedback && curFeedback == i) // if already showing feedback
                {
                    showFeedback = false;
                }
                else // not showing feedback
                {
                    curFeedback = i;
                    showFeedback = true;
                    feedbackText = history[curFeedback];
                    windowTitle = history[curFeedback].Split('\n')[0];
                }
            }
        }
        GUILayout.EndVertical();

        // All columns are drawn
        GUILayout.EndHorizontal();

        // Don't forget to end the area
        GUILayout.EndArea();
    }


    // Step Attempt History Window, Code borrowed from console
    string windowTitle = "";
    Rect windowRect = new Rect(0,0,Screen.width * 0.5f,Screen.height * 0.5f);
    bool showFeedback = false;
    string feedbackText = "";
    int curFeedback = -1;
    Vector2 scrollPosition = Vector2.zero;
    void windowFunc(int id)
    {
        // dimension variables
        float sideBuffer = 13;
        float topBuffer = 26;
        float bottomBuffer = 14;
        float winWidth = windowRect.width - sideBuffer * 2;
        float vScrollWidth = 18;
        float width = winWidth - vScrollWidth;
        //float height = GUI.skin.box.CalcHeight(new GUIContent(feedbackText), width);
        float height = GUI.skin.label.CalcHeight(new GUIContent(feedbackText), width);
        
        // taking the scrollbar into account
        Rect scrollRect = new Rect(sideBuffer, topBuffer, winWidth, windowRect.height - topBuffer - bottomBuffer);
        windowRect.height = Mathf.Min(height + topBuffer + bottomBuffer, Screen.height * 0.5f);
        if (windowRect.height < Screen.height * 0.5f)
        {
            width += vScrollWidth; // hide the scroll bar
        }

        // Console Background
        GUI.backgroundColor = Color.black;
        GUI.Box(scrollRect, "");
        GUI.backgroundColor = Color.white; // white again for vertical scroll bar

        // begin scroll view
        scrollPosition = GUI.BeginScrollView(
                scrollRect, // scroll rectangle
                scrollPosition,
                new Rect(0, 0, width, height) // view inside scroll rectangle
                );
        //GUI.Box(new Rect(0, 0, width, height), feedbackText);
        GUI.skin.label.wordWrap = true;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUI.skin.label.normal.textColor = Color.cyan;
        GUI.Label(new Rect(0, 0, width, height), feedbackText.Replace("\n","\n >> "));

        // end scroll view and make window draggable
        GUI.EndScrollView();
        
        // X button to close window
        float btnLength = 21;
        GUI.skin.button.fixedHeight = btnLength;
        if (GUI.Button(new Rect(windowRect.width - sideBuffer - btnLength, topBuffer-btnLength, btnLength, btnLength), "X"))
        {
            showFeedback = false;
        }

        GUI.DragWindow();
    }
}
