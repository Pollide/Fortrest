/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : ResourceGen.cs
Description : This script manages the window and functions to create your debug log
Author :  Allister Hamilton / Pol Idelon / Cory Marsh

**************************************************************************/

using UnityEngine;

#if UNITY_EDITOR //Unity can throw an error if it compiles UnityEditor, which is not possible in a standalone build.
using UnityEditor;
using UnityEditor.AnimatedValues; //to play animations in the window
using System.IO; //to access in and out filing

namespace ThirdClass.ResourceGen //creating a namespace helps keep it apart when dealing with many other packages
{
    public class ResourceGen : EditorWindow //to access the editor features, change MonoBehaviour to this
    {
        string InputString = "";
        static string CustomPath = "Assets/ResourceGen/";
        static string SavedFile = "SaveData.txt";


        GUISkin skin;
        AnimBool AnimatedValue;
        private GenerateList GeneratedList;
        private GameObject newResourcePrefab; // Temporary variable to store the newly added resource prefab
        Texture2D backgroundTexture; // Background color
        Rect background; // Background size

        bool generationSucessful = true;
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
            GeneratedList = new GenerateList();
            skin = Resources.Load<GUISkin>("WindowSkins/ResourceGeneratorSkin");

        }

        //set the global font for the window
        void SetFont(string name = null)
        {
            //if font is null it will default to unity default arial
            GUI.skin.font = (name == null ? null : (Font)AssetDatabase.LoadAssetAtPath(CustomPath + name + ".otf", typeof(Font)));
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }
        public void CreateButton(string prefabNameString)
        {
            GUIContent buttonContent = new GUIContent((Texture)Resources.Load("WindowImages/" + prefabNameString));

            GameObject chosenPrefab = Resources.Load<GameObject>("WindowPrefabs/" + prefabNameString);

            GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button);

            customButtonStyle.normal.background = MakeTexture(100, 100, chosenPrefab == newResourcePrefab ? Color.red : Color.grey);
            customButtonStyle.fixedWidth = 100;
            customButtonStyle.fixedHeight = 100;


            // Debug.Log(newResourcePrefab + "== " + chosenPrefab);

            if (GUILayout.Button(buttonContent, customButtonStyle))
            {
                newResourcePrefab = chosenPrefab;
            }
        }
        //runs when the GUI is refreshed
        private void OnGUI()
        {
            minSize = new Vector2(530, 290); //prevents user shrinking so small it cuts off title

            // COLORS
            backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, new Color(0.1f, 0.7f, 0.2f, 1));
            backgroundTexture.Apply();
            background.x = 0;
            background.y = 0;
            background.width = Screen.width;
            background.height = Screen.height;
            GUI.DrawTexture(background, backgroundTexture);
            GUI.backgroundColor = Color.green;

            // SAVING DATA
            bool previous = RunOnStart;
            RunOnStart = EditorGUILayout.ToggleLeft("Spawn resources accross whole terrain", RunOnStart);
            if (RunOnStart != previous) //only run if the toggle has been pressed
            {
                Repaint(); //updates the window
                EditorUtility.SetDirty(this); //tells unity this has been changed
                _RunOnStart = RunOnStart; //set so it can serialise
                var json = JsonUtility.ToJson(this);
                File.WriteAllText(CustomPath + SavedFile, json);
                AssetDatabase.Refresh(); //so the file is visible
            }

            GUI.contentColor = Color.white; //sets the text to white
            // DRAWING TITLE
            EditorGUILayout.BeginHorizontal();

            GUIStyle customLabelStyle = new GUIStyle(skin.GetStyle("Sexy"));
            customLabelStyle.normal.textColor = Color.white; // Set the desired text color

            GUILayout.Label("RESOURCE GENERATOR", customLabelStyle);
            EditorGUILayout.EndHorizontal();

            // IMAGE BUTTONS
            EditorGUILayout.BeginHorizontal(); // Side by side layout

            CreateButton("Boulder");
            CreateButton("Bush");
            CreateButton("Tree");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");

            //EditorGUILayout.LabelField("Resource Prefabs");

            //for (int i = 0; i < GeneratedList.resourcePrefabs.Count; i++)
            //{
            //    GeneratedList.resourcePrefabs[i] = EditorGUILayout.ObjectField("Resource " + (i + 1), GeneratedList.resourcePrefabs[i], typeof(GameObject), false) as GameObject;
            //}

            //newResourcePrefab = EditorGUILayout.ObjectField("Add New Resource", newResourcePrefab, typeof(GameObject), false) as GameObject;
            GeneratedList.resourcePrefab = newResourcePrefab;

            if (GeneratedList.resourcePrefab == null)
            {
                EditorGUILayout.TextArea("Select a resource to begin", ReturnGUIStyle(20));

                EditorGUI.BeginDisabledGroup(true);
            }
            GeneratedList.numberOfResources = EditorGUILayout.IntField("Number of Resources", GeneratedList.numberOfResources);

            GeneratedList.rangeWidth = EditorGUILayout.FloatField("Spawn area X", GeneratedList.rangeWidth);

            GeneratedList.rangeHeight = EditorGUILayout.FloatField("Spawn area Z", GeneratedList.rangeHeight);

            GeneratedList.minY = EditorGUILayout.FloatField("Lowest spawn height", GeneratedList.minY);

            if (GeneratedList.minY < 0)
            {
                EditorGUILayout.TextArea("Below zero is sea level!", ReturnGUIStyle(15));
            }

            GeneratedList.maxY = EditorGUILayout.FloatField("Highest spawn height", GeneratedList.maxY);

            //if (GUILayout.Button("Change Resources", ReturnGUIStyle(30, "button")))
            //{
            //    GUIUtility.systemCopyBuffer = "if (System.DateTime.Now.Second + 10 > Time.realtimeSinceStartup)"; //example
            //
            //    InputString = ""; //cleatrs int the input text
            //}



            if (GUILayout.Button("Generate", ReturnGUIStyle(30, "button")))
            {
                generationSucessful = GeneratedList.GenerateResources();
            }

            if (!generationSucessful)
            {
                EditorGUILayout.TextArea("No suitable area to generate with the values you have given!", ReturnGUIStyle(15));
            }

            if (GUILayout.Button("Delete", ReturnGUIStyle(30, "button")))
            {
                GeneratedList.ClearResourceList();
            }

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