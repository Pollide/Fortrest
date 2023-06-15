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
using System.Reflection;
using System;

public class ResourceGenerator : EditorWindow // To access the editor features, change MonoBehaviour to this
{
    // String variables for file paths and error display
    static string CustomPath = "Assets/ResourceGen/";
    public static ResourceGenerator global;
    // Resource generation variables
    private GenerateList GeneratedList; // Resource list object
    private GameObject newResourcePrefab; // Temporary variable to store the newly added resource prefab
    private GameObject editorBox;
    private bool generationSucessful = true; // Confirmation bool

    bool resourceSelected = false;
    bool terrainSelected = false;
    bool terrainToggleSelected = false;
    bool areaToggleSelected = false;

    // Visual variables
    GUISkin skin; // Skin variable
    AnimBool AnimatedValue; // Animation variable
    AnimBool AnimatedValue2; // Animation variable for terrain
    AnimBool AnimatedValue3;  //for area of denial
    Texture2D backgroundTexture; // Background color
    Rect background; // Background size
    static float minX = 530.0f;
    static float minY = 210.0f;
    float windowXsize;
    float windowYsize;

    // Not currently used
    float currentHeight = 0.0f;
    bool valueStored = false;

    // Sound variables;
    public AudioClip clickSound;
    public AudioClip click2Sound;
    public AudioClip introSound;
    public AudioClip createSound;
    public AudioClip destroySound;
    public AudioClip closeSound;
    public bool boolChanged = false;
    public bool boolChanged2 = false;

    enum WindowSizeEnum
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge
    };
    WindowSizeEnum currentWindowSize;

    List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

    [MenuItem("Tools/Resource Generator")] // Adds access to the window through the toolbar

    // Function being called when opening the window
    public static void ShowWindow()
    {
        ResourceGenerator window = (ResourceGenerator)GetWindow(typeof(ResourceGenerator)); // Sets the title of the window
        window.minSize = new Vector2(minX, minY); // Minimal window size
        window.maxSize = new Vector2(minX, minY);
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
        AnimatedValue = new AnimBool(false);
        AnimatedValue.valueChanged.AddListener(Repaint);
        AnimatedValue2 = new AnimBool(false);
        AnimatedValue2.valueChanged.AddListener(Repaint);
        AnimatedValue3 = new AnimBool(false);
        AnimatedValue3.valueChanged.AddListener(Repaint);
        GeneratedList = new GenerateList();
        CreateFolders();
        global = this;
        TryGetUnityObjectsOfTypeFromPath(ReturnDirectPath(), assets);

        skin = Resources.Load<GUISkin>("Generator/ResourceGeneratorSkin");
        editorBox = Resources.Load<GameObject>("Generator/ResourceGenDisplayBox");
        GameObject editBox = PrefabUtility.InstantiatePrefab(editorBox) as GameObject;
        editorBox = editBox;
        clickSound = Resources.Load<AudioClip>("Generator/Sound/Click");
        click2Sound = Resources.Load<AudioClip>("Generator/Sound/Click2");
        introSound = Resources.Load<AudioClip>("Generator/Sound/Intro");
        createSound = Resources.Load<AudioClip>("Generator/Sound/Create");
        destroySound = Resources.Load<AudioClip>("Generator/Sound/Destroy");
        closeSound = Resources.Load<AudioClip>("Generator/Sound/Close");

        windowXsize = minX;
        windowYsize = minY;

        PlayClip(introSound);
    }

    // Window editing
    private void OnGUI()
    {
        SetColors();
        DrawTitles();

        if (!Terrain.activeTerrain)
        {
            GUILayout.Label("You need a terrain to begin!", ReturnGUIStyle(30, "", Color.red));
            return;
        }
        Terrain.activeTerrain.gameObject.layer = LayerMask.NameToLayer("Terrain");

        PlaceButtons();
        CheckSelected();
        ChangeWindowSize();
        PlaySounds();

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
        PlayClip(closeSound);
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
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        foreach (var asset in assets)
        {
            CreateButton(asset.name);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        AnimatedValue2.target = EditorGUILayout.ToggleLeft("Terrain?", AnimatedValue2.target, skin.GetStyle("Sexy3"));

        if (EditorGUILayout.BeginFadeGroup(AnimatedValue2.faded))
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select a terrain type", skin.GetStyle("Sexy2"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            SetTerrainTextures();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
        EditorGUILayout.EndFadeGroup();

        AnimatedValue3.target = EditorGUILayout.ToggleLeft("Area of denial?", AnimatedValue3.target, skin.GetStyle("Sexy3"));


        if (EditorGUILayout.BeginFadeGroup(AnimatedValue3.faded))
        {  // Define the list of options for the dropdown

            List<string> options = new List<string>();

            for (int i = 0; i < Terrain.activeTerrain.terrainData.terrainLayers.Length; i++)
            {
                options.Add(Terrain.activeTerrain.terrainData.terrainLayers[i].name);
            }

            if (GeneratedList.TerrainTextureDenial == -1)
            {
                GeneratedList.TerrainTextureDenial = 0;
            }

            // Create the dropdown using GUILayout
            GUILayout.Label("Texture to generate below your resource", ReturnGUIStyle(20));
            GeneratedList.TerrainTextureDenial = GUILayout.SelectionGrid(GeneratedList.TerrainTextureDenial, options.ToArray(), 1);
            GUILayout.Space(10);
            GUILayout.Label("Radius", ReturnGUIStyle(20));
            GeneratedList.AreaOfDenialRadius = GUILayout.HorizontalSlider(GeneratedList.AreaOfDenialRadius, 1f, 5.0f);
        }
        else
        {
            GeneratedList.TerrainTextureDenial = -1;
        }

        EditorGUILayout.EndFadeGroup();

        GUILayout.Space(20);
    }

    void SetTerrainTextures()
    {
        Terrain terrain = Terrain.activeTerrain;
        for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
        {
            CreateButton(terrain.terrainData.terrainLayers[i].diffuseTexture.name, terrain.terrainData.terrainLayers[i].diffuseTexture);
        }
    }

    ///<summary>
    ///Creates the input fields, checks validity of input, generates resources
    ///</summary>
    void ParametersAndGeneration()
    {
        if (EditorGUILayout.BeginFadeGroup(AnimatedValue.faded))
        {
            EditorGUILayout.BeginVertical("box");

            GeneratedList.resourcePrefab = newResourcePrefab;

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
                PlayClip(createSound);
            }

            if (!generationSucessful)
            {
                EditorGUILayout.TextArea("No suitable area to generate with the values you have given!", ReturnGUIStyle(15, color: Color.red));
            }

            if (GUILayout.Button("Delete", ReturnGUIStyle(30, "button")))
            {
                GeneratedList.ClearResourceList();
                PlayClip(destroySound);
            }
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndFadeGroup();

        GUIStyle customButtonStyle = new GUIStyle();
        customButtonStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("Regenerate Terrain Textures (20s)", ReturnGUIStyle(30, "button")))
        {
            GenerateTerrainTextures();
        }

    }

    private float GetTerrainHeight(Vector3 position)
    {
        float height = Terrain.activeTerrain.SampleHeight(position);

        // Adjust height based on terrain position and size
        height = height * Terrain.activeTerrain.terrainData.size.y / Terrain.activeTerrain.terrainData.heightmapResolution;

        return height;
    }

    void GenerateTerrainTextures()
    {
        int callsInt = 0;

        GeneratedList.AreaOfDenialRadius = 10;

        for (int x = 0; x < Terrain.activeTerrain.terrainData.size.x; x += (int)GeneratedList.AreaOfDenialRadius)
        {
            for (int z = 0; z < Terrain.activeTerrain.terrainData.size.z; z += (int)GeneratedList.AreaOfDenialRadius)
            {
                Vector3 position = new Vector3(x, 0, z);
                Vector3 worldPosition = Terrain.activeTerrain.GetPosition() + position;

                callsInt++;

                if (callsInt > 1000) //prevent stack overflow
                {
                    {
                        return;
                    }
                }

                GeneratedList.TerrainTextureDenial = 0;

                if (GetTerrainHeight(worldPosition) > 5)
                {
                    GeneratedList.TerrainTextureDenial = 1;

                }
                if (GetTerrainHeight(worldPosition) > 20)
                {
                    GeneratedList.TerrainTextureDenial = 2;
                }

                if (GetTerrainHeight(worldPosition) > 30) //snow
                {
                    GeneratedList.TerrainTextureDenial = 3;
                }

                GeneratedList.ChangeTerrainTexture(worldPosition);

            }
        }

        GeneratedList.AreaOfDenialRadius = 2;
        GeneratedList.TerrainTextureDenial = -1;
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

        if (!terrainBool && !texture)
            buttonContent.text = prefabNameString;

        buttonContent.tooltip = prefabNameString;


        GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button);

        bool selectedBool;

        if (terrainBool)
        {
            selectedBool = GeneratedList.SelectTexturesList.Contains(texture);
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
        customButtonStyle.padding = new RectOffset(5, 5, 8, 5);

        if (GUILayout.Button(buttonContent, customButtonStyle))
        {
            if (terrainBool)
            {
                if (selectedBool)
                {
                    GeneratedList.SelectTexturesList.Remove(texture);
                    StopAllClips();
                    PlayClip(click2Sound);
                }
                else
                {
                    GeneratedList.SelectTexturesList.Add(texture);
                    StopAllClips();
                    PlayClip(click2Sound);
                }
            }
            else
            {
                if (selectedBool)
                {
                    newResourcePrefab = null;
                    resourceSelected = false;
                    StopAllClips();
                    PlayClip(click2Sound);
                }
                else
                {
                    newResourcePrefab = Resources.Load<GameObject>(ReturnPathPath() + prefabNameString);
                    resourceSelected = true;
                    StopAllClips();
                    PlayClip(click2Sound);
                }


            }
        }
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

    void CheckSelected()
    {
        // Checking if the tickbox is toggled. Could bypass this step but decided to use a more readable boolean
        if (AnimatedValue2.target == true)
        {
            terrainToggleSelected = true;
        }
        else
        {
            terrainToggleSelected = false;
        }

        // Checking if the tickbox is toggled.
        if (AnimatedValue3.target == true)
        {
            areaToggleSelected = true;
        }
        else
        {
            areaToggleSelected = false;
        }

        // If at least one texture button is selected then this boolean is true
        if (GeneratedList.SelectTexturesList.Count > 0)
        {
            terrainSelected = true;
        }
        else
        {
            terrainSelected = false;
        }

        // If the tickbox is toggled, it requires a texture to be also selected to display the parameters
        if (terrainToggleSelected)
        {
            if (terrainSelected) // If texture is selected
            {
                AnimatedValue.target = resourceSelected;
            }
            else
            {
                AnimatedValue.target = false;
            }
        }
        else // If the tickbox is not toggled or untoggled, display the parameters if a resource is toggled
        {
            AnimatedValue.target = resourceSelected; // Displays if resourceSelected is true

            if (GeneratedList.SelectTexturesList.Count > 0) // If a texture was selected when the tickbox was untoggled
            {
                for (int i = 0; i < GeneratedList.SelectTexturesList.Count; i++) // Unselect all textures
                {
                    GeneratedList.SelectTexturesList.Remove(GeneratedList.SelectTexturesList[i]);
                }
            }
        }
    }

    void ChangeWindowSize()
    {
        if (AnimatedValue.target == false && terrainToggleSelected == false)
        {
            currentWindowSize = WindowSizeEnum.Tiny;
        }
        else if (AnimatedValue.target == false && terrainToggleSelected == true)
        {
            currentWindowSize = WindowSizeEnum.Small;
        }
        else if (AnimatedValue.target == true && terrainToggleSelected == false)
        {
            currentWindowSize = WindowSizeEnum.Medium;
        }
        else if (AnimatedValue.target == true && terrainToggleSelected == true)
        {
            currentWindowSize = WindowSizeEnum.Large;
        }

        if (currentWindowSize == WindowSizeEnum.Tiny)
        {
            windowYsize = 210.0f;
        }
        else if (currentWindowSize == WindowSizeEnum.Small)
        {
            windowYsize = 355.0f;
        }
        else if (currentWindowSize == WindowSizeEnum.Medium)
        {
            windowYsize = 465.0f;
        }
        else if (currentWindowSize == WindowSizeEnum.Large)
        {
            windowYsize = 600.0f;
        }

        if (Screen.height - 27.0f != windowYsize)
        {
            minSize = new Vector2(windowXsize, windowYsize);
            maxSize = new Vector2(windowXsize, windowYsize);
        }
    }

    public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
            null
        );

        method.Invoke(
            null,
            new object[] { clip, startSample, loop }
        );
    }

    public static void StopAllClips()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllPreviewClips",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { },
            null
        );

        method.Invoke(
            null,
            new object[] { }
        );
    }

    void PlaySounds()
    {
        if (boolChanged != terrainToggleSelected)
        {
            StopAllClips();
            PlayClip(clickSound);
            boolChanged = terrainToggleSelected;
        }

        if (boolChanged2 != areaToggleSelected)
        {
            StopAllClips();
            PlayClip(clickSound);
            boolChanged2 = areaToggleSelected;
        }
    }
}
#endif // The end of the unity editor checker