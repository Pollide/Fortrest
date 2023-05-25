/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : ResourceGen.cs
Description : This script manages the window and functions to create your debug log
Author :  Allister Hamilton / Pol Idelon / Cory Marsh
Mail : allister.hamilton @mds.ac.nz
**************************************************************************/

using UnityEngine;

#if UNITY_EDITOR //Unity can throw an error if it compiles UnityEditor, which is not possible in a standalone build.
using UnityEditor;
using UnityEditor.AnimatedValues; //to play animations in the window
using System.IO; //to access in and out filing

namespace Allister.DebugTool //creating a namespace helps keep it apart when dealing with many other packages
{
    public class ResourceGen : EditorWindow //to access the editor features, change MonoBehaviour to this
    {
        AnimBool AnimatedValue;
        string InputString = "";
        static string CustomPath = "Assets/ResourceGen/";
        static string SavedFile = "SaveData.txt";
        bool StartupRun;
        private GenerateList GeneratedList;
        private GameObject newResourcePrefab; // Temporary variable to store the newly added resource prefab

        // Image buttons
        private Texture buttonTexture;
        private GUIContent buttonContent;

        GUISkin skin;

        Texture2D backgroundTexture; // Background color
        Rect background; // Background size

        private static bool RunOnStart;
        [SerializeField] private bool _RunOnStart;

        [MenuItem("Tools/Resource Generator")] //makes it appear in the top unity menu dropdown

        public static void ShowWindow()
        {
            ResourceGen window = (ResourceGen)GetWindow(typeof(ResourceGen), true, "Resource Generator"); //sets the title of the window

            if (File.Exists(CustomPath + SavedFile)) //check the file exists to prevent error
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(CustomPath + SavedFile), window);
                RunOnStart = window._RunOnStart; //loads the value
            }

            window.Show(); //show the window
        }

        GUIStyle ReturnGUIStyle(int fontSize = 30, string type = "")
        {
            GUIStyle headStyle = type == "" ? new GUIStyle() : new GUIStyle(type); //only input a type if it has a value, else skip and just return the default GUIStyle()
            headStyle.fontSize = fontSize;
            headStyle.normal.textColor = Color.white;

            return headStyle;
        }

        //runs the frame it is enabled
        private void OnEnable()
        {
            AnimatedValue = new AnimBool(false);
            AnimatedValue.valueChanged.AddListener(Repaint); //add a listener so it can detect when repait occurs and can fade properly
            GeneratedList = new()
            skin = Resources.Load<GUISkin>("WindowSkins/ResourceGeneratorSkin");
        }

        //set the global font for the window
        void SetFont(string name = null)
        {
            //if font is null it will default to unity default arial
            GUI.skin.font = (name == null ? null : (Font)AssetDatabase.LoadAssetAtPath(CustomPath + name + ".otf", typeof(Font)));
        }
 
        //runs when the GUI is refreshed
        private void OnGUI()
        {
            minSize = new Vector2(530, 290); //prevents user shrinking so small it cuts off title

            // COLORS
            backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, new Color(0.7f, 0.1f, 0.2f, 1));
            backgroundTexture.Apply();
            background.x = 0;
            background.y = 0;
            background.width = Screen.width;
            background.height = Screen.height;
            GUI.DrawTexture(background, backgroundTexture);
            GUI.backgroundColor = Color.green;

            // SAVING DATA
            bool previous = RunOnStart;
            RunOnStart = EditorGUILayout.ToggleLeft("Terrain Auto Generate", RunOnStart);
            if (RunOnStart != previous) //only run if the toggle has been pressed
            {
                Repaint(); //updates the window
                EditorUtility.SetDirty(this); //tells unity this has been changed
                _RunOnStart = RunOnStart; //set so it can serialise
                var json = JsonUtility.ToJson(this);
                File.WriteAllText(CustomPath + SavedFile, json);
                AssetDatabase.Refresh(); //so the file is visible
            }

            // DRAWING TITLE
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("RESOURCE GENERATOR", skin.GetStyle("Sexy"));
            EditorGUILayout.EndHorizontal();

            // IMAGE BUTTONS
            EditorGUILayout.BeginHorizontal(); // Side by side layout

            buttonTexture = (Texture)Resources.Load("WindowImages/Boulder");
            buttonContent = new GUIContent(buttonTexture);
            if (GUILayout.Button(buttonContent, GUILayout.Width(100), GUILayout.Height(100)))
            {
                newResourcePrefab = Resources.Load<GameObject>("WindowPrefabs/Stone");
            }
            
            buttonTexture = (Texture)Resources.Load("WindowImages/Tree");
            buttonContent = new GUIContent(buttonTexture);
            if (GUILayout.Button(buttonContent, GUILayout.Width(100), GUILayout.Height(100)))
            {
                newResourcePrefab = Resources.Load<GameObject>("WindowPrefabs/Wood 02");
            }

            buttonTexture = (Texture)Resources.Load("WindowImages/Bush");
            buttonContent = new GUIContent(buttonTexture);
            if (GUILayout.Button(buttonContent, GUILayout.Width(100), GUILayout.Height(100)))
            {
                newResourcePrefab = Resources.Load<GameObject>("WindowPrefabs/Wood 01");
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");

            GUI.contentColor = Color.white; //sets the text to white

            //EditorGUILayout.LabelField("Resource Prefabs");

            //for (int i = 0; i < GeneratedList.resourcePrefabs.Count; i++)
            //{
            //    GeneratedList.resourcePrefabs[i] = EditorGUILayout.ObjectField("Resource " + (i + 1), GeneratedList.resourcePrefabs[i], typeof(GameObject), false) as GameObject;
            //}

            //newResourcePrefab = EditorGUILayout.ObjectField("Add New Resource", newResourcePrefab, typeof(GameObject), false) as GameObject;

            if (newResourcePrefab != null)
            {               
                GeneratedList.resourcePrefab = newResourcePrefab;
                newResourcePrefab = null;
            }

            GeneratedList.numberOfResources = EditorGUILayout.IntField("Number of Resources", GeneratedList.numberOfResources);

            GeneratedList.rangeWidth = EditorGUILayout.FloatField("Spawn area X", GeneratedList.rangeWidth);

            GeneratedList.rangeHeight = EditorGUILayout.FloatField("Spawn area Z", GeneratedList.rangeHeight);

            //if (GUILayout.Button("Change Resources", ReturnGUIStyle(30, "button")))
            //{
            //    GUIUtility.systemCopyBuffer = "if (System.DateTime.Now.Second + 10 > Time.realtimeSinceStartup)"; //example
            //
            //    InputString = ""; //cleatrs int the input text
            //}

            if(GUILayout.Button("Generate", ReturnGUIStyle(30, "button")))
            {
                GeneratedList.GenerateResources();
            }
            StartupRun = true;//first frame tick this as true, so then if you tick 'auto run' it wont run instantly after being toggled, it will instead run after next time the window is awakened

            SetFont(); //set the arial font as it is easier to read
            EditorGUI.EndDisabledGroup(); //stop the inspector being greyed out


            AnimatedValue.target = InputString != ""; //animate the input to appear
            if (AnimatedValue.target)
            {
                EditorGUILayout.BeginFadeGroup(AnimatedValue.faded); //run the animation
                EditorGUILayout.TextArea("Inputed: " + InputString, ReturnGUIStyle(20));
                EditorGUILayout.EndFadeGroup(); //stop the animation
            }

            //EditorGUILayout.TextArea("Current Scene: " + UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name, ReturnGUIStyle(20)); //display what is currently in the system clipboard

            EditorGUILayout.EndVertical();
        }
    }
}
#endif //the end of the unity editor checker