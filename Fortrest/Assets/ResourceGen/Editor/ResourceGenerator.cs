/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2023 Media Design School
File Name : ResourceGen.cs
Description : This script manages the window and functions to create your debug log
Author :  Allister Hamilton && Pol Idelon && Cory Marsh

**************************************************************************/

using UnityEngine;

#if UNITY_EDITOR // Unity can throw an error if it compiles UnityEditor, which is not possible in a standalone build
using UnityEditor;
using UnityEditor.AnimatedValues; // To play animations in the window
using System.IO; // To access in and out filing
using System.Collections.Generic;
public class ResourceGenerator : EditorWindow // To access the editor features, change MonoBehaviour to this
{
    // String variables for file paths and error display
    static string CustomPath = "Assets/ResourceGen/";

    // Resource generation variables
    private GenerateList GeneratedList; // Resource list object
    private GameObject newResourcePrefab; // Temporary variable to store the newly added resource prefab
    private GameObject editorBox;
    private bool generationSucessful = true; // Confirmation bool
    private bool biomeWide = false;
    bool AutoAddAllTerrainTextures = true;
    public List<Texture> SelectTexturesList = new List<Texture>();
    // Visual variables
    GUISkin skin; // Skin variable
    //AnimBool AnimatedValue; // Animation variable     
    Texture2D backgroundTexture; // Background color
    Rect background; // Background size
    private static float minX = 530.0f;
    private static float minY = 400.0f;
    List<Object> assets = new List<Object>();
    // UNSURE
    // private static bool RunOnStart; // UNSURE
    // [SerializeField] private bool _RunOnStart; // UNSURE

    [MenuItem("Tools/Resource Generator")] // Adds access to the window through the toolbar

    // Function being called when opening the window
    public static void ShowWindow()
    {
        ResourceGenerator window = (ResourceGenerator)GetWindow(typeof(ResourceGenerator)); // Sets the title of the window
        window.minSize = new Vector2(minX, minY); // Minimal window size
        window.maxSize = new Vector2(minX * 1.5f, minY * 1.5f); // Maximal window size

        //if (File.Exists(CustomPath + SavedFile)) // Checks if the file exists to prevent error
        //{
        //    JsonUtility.FromJsonOverwrite(File.ReadAllText(CustomPath + SavedFile), window);
        //    // RunOnStart = window._RunOnStart; // Loads the value
        //}
        //window.Show(); // Displays the window
    }

    public static int TryGetUnityObjectsOfTypeFromPath<T>(string path, List<T> assetsFound) where T : UnityEngine.Object
    {
        string[] filePaths = System.IO.Directory.GetFiles(path);

        int countFound = 0;

        if (filePaths != null && filePaths.Length > 0)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(T));
                if (obj is T asset)
                {
                    countFound++;
                    if (!assetsFound.Contains(asset))
                    {
                        assetsFound.Add(asset);
                    }
                }
            }
        }

        return countFound;
    }

    void CreateFolders()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(ReturnDirectPath());

        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
            AssetDatabase.Refresh();
        }


        directoryInfo = new DirectoryInfo(ReturnIconPath());

        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
            AssetDatabase.Refresh();
        }
    }

    // Function called when the editor window first opens
    private void OnEnable()
    {
        // Initialising variables
        //AnimatedValue = new AnimBool(false);
        //AnimatedValue.valueChanged.AddListener(Repaint); // Adding listener for fading animation
        GeneratedList = new GenerateList();
        CreateFolders();

        TryGetUnityObjectsOfTypeFromPath(ReturnDirectPath(), assets);

        skin = Resources.Load<GUISkin>("Generator/ResourceGeneratorSkin");
        editorBox = Resources.Load<GameObject>("Generator/ResourceGenDisplayBox");
        GameObject editBox = PrefabUtility.InstantiatePrefab(editorBox) as GameObject;
        editorBox = editBox;
        //editorBox.transform.position = GeneratedList.CalculatePosition();
        //editorBox.transform.position = new Vector3 (editorBox.transform.position.x, GeneratedList.minY + ((GeneratedList.maxY - GeneratedList.minY) / 2), editorBox.transform.position.z);
        //editorBox.transform.localScale = new Vector3(GeneratedList.rangeWidth, GeneratedList.maxY - GeneratedList.minY, GeneratedList.rangeHeight);
    }

    private void OnFocus()
    {
        //editorBox.transform.position = GeneratedList.CalculatePosition();
        //editorBox.transform.position = new Vector3(editorBox.transform.position.x, GeneratedList.minY + ((GeneratedList.maxY - GeneratedList.minY) / 2), editorBox.transform.position.z);
        //editorBox.transform.localScale = new Vector3(GeneratedList.rangeWidth, GeneratedList.maxY - GeneratedList.minY, GeneratedList.rangeHeight);
    }

    // Window editing
    private void OnGUI()
    {
        SetColors();
        DrawTitles();
        //SetFont();

        if (!Terrain.activeTerrain)
        {
            GUILayout.Label("You need a terrain to begin!", ReturnGUIStyle(30, "", Color.red));
            return;
        }

        Terrain.activeTerrain.gameObject.layer = LayerMask.NameToLayer("Terrain");

        PlaceButtons();
        ParametersAndGeneration();
        editorBox.transform.position = GeneratedList.CalculatePosition();
        editorBox.transform.position = new Vector3(editorBox.transform.position.x, GeneratedList.minY + ((GeneratedList.maxY - GeneratedList.minY) / 2), editorBox.transform.position.z);
        editorBox.transform.localScale = new Vector3(GeneratedList.rangeWidth, GeneratedList.maxY - GeneratedList.minY, GeneratedList.rangeHeight);
    }

    ///<summary>
    ///Sets all of the elements' colors
    ///</summary>
    void SetColors()
    {
        // Setting colors
        GUI.backgroundColor = Color.green;
        GUI.contentColor = Color.white;
        backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(0.1f, 0.7f, 0.2f, 1));
        backgroundTexture.Apply();
        background.x = 0;
        background.y = 0;
        background.width = Screen.width;
        background.height = Screen.height;
        GUI.DrawTexture(background, backgroundTexture);
    }

    ///<summary>
    ///Creates and draws the titles
    ///</summary>
    void DrawTitles()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("RESOURCE GENERATOR", skin.GetStyle("Sexy"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Select a resource to begin", skin.GetStyle("Sexy2"));
        EditorGUILayout.EndHorizontal();
    }

    private void OnDisable()
    {
        DestroyImmediate(editorBox);
    }

    ///<summary>
    ///Sets the font for the text
    ///</summary>
    void SetFont(string name = null)
    {
        // If font is null it will default to unity default arial
        GUI.skin.font = (name == null ? null : (Font)AssetDatabase.LoadAssetAtPath(CustomPath + name + ".otf", typeof(Font)));
    }

    ///<summary>
    ///Calls the create button function for each resource, which creates a button with an image
    ///</summary>
    void PlaceButtons()
    {
        GUIStyle customButtonStyle = new GUIStyle();
        customButtonStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();


        foreach (var asset in assets)
        {
            CreateButton(asset.name);

        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Select a terrain type", skin.GetStyle("Sexy2"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        // CreateButton("Sand");
        // CreateButton("Grass");
        //CreateButton("Dirt");

        SetTerrainTextures();
        AutoAddAllTerrainTextures = false; //on the first frame, the terrain buttons should all be selected on default
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    void SetTerrainTextures()
    {
        Terrain terrain = Terrain.activeTerrain;
        for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
        {
            // Debug.Log("i: " + terrain.terrainData.terrainLayers[i]);
            CreateButton(terrain.terrainData.terrainLayers[i].diffuseTexture.name, terrain.terrainData.terrainLayers[i].diffuseTexture);
        }
    }

    ///<summary>
    ///Creates the input fields, checks validity of input, generates resources
    ///</summary>
    void ParametersAndGeneration()
    {
        EditorGUILayout.BeginVertical("box");

        GeneratedList.resourcePrefab = newResourcePrefab;

        if (GeneratedList.resourcePrefab == null)
        {
            EditorGUI.BeginDisabledGroup(true);
        }

        biomeWide = EditorGUILayout.ToggleLeft("Spawn resources accross whole biome", biomeWide);

        GeneratedList.numberOfResources = EditorGUILayout.IntField("Number of Resources", GeneratedList.numberOfResources);

        GeneratedList.rangeWidth = EditorGUILayout.FloatField("Spawn area X", GeneratedList.rangeWidth);

        GeneratedList.rangeHeight = EditorGUILayout.FloatField("Spawn area Z", GeneratedList.rangeHeight);

        GeneratedList.positionOnTerrain = EditorGUILayout.Vector2Field("Position on terrain", GeneratedList.positionOnTerrain);

        GeneratedList.minDistance = EditorGUILayout.FloatField("Distance between objects", GeneratedList.minDistance);

        GeneratedList.minY = EditorGUILayout.FloatField("Lowest spawn height", GeneratedList.minY);

        if (GeneratedList.minY < 0)
        {
            EditorGUILayout.TextArea("Below zero is sea level!", ReturnGUIStyle(15, color: Color.red));
        }


        GeneratedList.maxY = EditorGUILayout.FloatField("Highest spawn height", GeneratedList.maxY);

        if (GUILayout.Button("Generate", ReturnGUIStyle(30, "button")))
        {
            generationSucessful = GeneratedList.GenerateResources();
        }

        if (!generationSucessful)
        {
            EditorGUILayout.TextArea("No suitable area to generate with the values you have given!", ReturnGUIStyle(15, color: Color.red));
        }

        if (SelectTexturesList.Count == 0 && Terrain.activeTerrain.terrainData.terrainLayers.Length > 0)
        {
            EditorGUILayout.TextArea("Nothing will generate as you haven't selected a terrain texture!", ReturnGUIStyle(15, color: Color.red));
        }

        if (GUILayout.Button("Delete", ReturnGUIStyle(30, "button")))
        {
            GeneratedList.ClearResourceList();
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndVertical();

        //Repaint(); // Redraws the window    
    }

    ///<summary>
    ///Creates and initialises texture
    ///</summary>
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

    ///<summary>
    ///Creates buttons with textures and a background behind them that changes color when selected
    ///</summary>
    ///

    public static string ReturnPathPath()
    {
        return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name + "/";
    }
    public static string ReturnDirectPath()
    {
        return "Assets/Resources/" + ReturnPathPath();
    }

    public static string ReturnIconPath()
    {
        return ReturnPathPath() + "Icons/";
    }
    public void CreateButton(string prefabNameString, Texture texture = null)
    {

        bool terrainBool = texture != null;

        if (!terrainBool)
        {
            texture = (Texture)Resources.Load(ReturnIconPath() + prefabNameString);
        }

        GUIContent buttonContent = new GUIContent(texture);
        buttonContent.text = prefabNameString;
        buttonContent.tooltip = prefabNameString;


        GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button);

        bool selectedBool;

        if (terrainBool)
        {
            if (AutoAddAllTerrainTextures)
            {
                SelectTexturesList.Add(texture);
            }
            selectedBool = SelectTexturesList.Contains(texture);
        }
        else
        {
            selectedBool = newResourcePrefab && newResourcePrefab.name.Contains(prefabNameString);
        }

        customButtonStyle.normal.background = MakeTexture(100, 100, selectedBool ? (new Color(0, 0.2f, 0)) : Color.grey);
        customButtonStyle.fixedWidth = 100;
        customButtonStyle.fixedHeight = 100;
        customButtonStyle.alignment = TextAnchor.MiddleCenter;
        customButtonStyle.imagePosition = ImagePosition.ImageAbove;
        customButtonStyle.padding = new RectOffset(2, 2, 10, 2);
        // EditorGUILayout.BeginVertical();
        if (GUILayout.Button(buttonContent, customButtonStyle))
        {
            if (terrainBool)
            {
                if (selectedBool)
                {
                    SelectTexturesList.Remove(texture);
                }
                else
                {
                    SelectTexturesList.Add(texture);
                }
            }
            else
            {
                newResourcePrefab = Resources.Load<GameObject>(ReturnPathPath() + prefabNameString);
            }
        }

        //  EditorGUILayout.EndVertical();
    }

    ///<summary>
    ///Changes the text style more easily
    ///</summary>
    GUIStyle ReturnGUIStyle(int fontSize = 30, string type = "", Color color = default)
    {
        GUIStyle headStyle = type == "" ? new GUIStyle() : new GUIStyle(type); //only input a type if it has a value, else skip and just return the default GUIStyle()
        headStyle.fontSize = fontSize;
        headStyle.normal.textColor = color == default ? Color.white : color;

        return headStyle;
    }

    //// SAVING DATA
    //bool previous = RunOnStart;
    //RunOnStart = EditorGUILayout.ToggleLeft("Spawn resources accross whole biome", RunOnStart);
    //if (RunOnStart != previous) //only run if the toggle has been pressed
    //{
    //    Repaint(); //updates the window
    //    EditorUtility.SetDirty(this); //tells unity this has been changed
    //    _RunOnStart = RunOnStart; //set so it can serialise
    //    var json = JsonUtility.ToJson(this);
    //    File.WriteAllText(CustomPath + SavedFile, json);
    //    AssetDatabase.Refresh(); //so the file is visible
    //}

    //AnimatedValue.target = InputString != ""; //animate the input to appear
    //if (AnimatedValue.target)
    //{
    //    EditorGUILayout.BeginFadeGroup(AnimatedValue.faded); //run the animation
    //    EditorGUILayout.TextArea("Inputed: " + InputString, ReturnGUIStyle(20));
    //    EditorGUILayout.EndFadeGroup(); //stop the animation
    //}
}
#endif // The end of the unity editor checker