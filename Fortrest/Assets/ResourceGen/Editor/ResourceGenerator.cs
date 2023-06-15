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
    // Making the script global
    public static ResourceGenerator global;

    // String variables for file paths and error display
    static string customPath = "Assets/ResourceGen/";  

    // Resource generation variables
    private GenerateList generatedList; // Resource list object
    private GameObject newResourcePrefab; // Temporary variable to store the newly added resource prefab
    private GameObject editorBox; // Object to display spawn area
    private bool generationSucessful = true; // Confirmation bool

    // Booleans to confirm every step, to display parameters and resize window accordingly 
    bool resourceSelected = false;
    bool terrainSelected = false;
    bool areaSelected = false;
    bool terrainToggleSelected = false;
    bool areaToggleSelected = false;

    // Visual variables
    GUISkin skin; // Skin variable
    AnimBool animatedValue; // Animation variable for parameters
    AnimBool animatedValue2; // Animation variable for terrain
    AnimBool animatedValue3;  // Animation variable for area of denial
    Texture2D backgroundTexture; // Background color
    Rect background; // Background size
    static float minX = 530.0f; // Window width
    static float minY = 281.0f; // Window height
    float windowXsize; // Modulable window width
    float windowYsize; // Modulable window height

    // Sound variables
    public AudioClip clickSound;
    public AudioClip click2Sound;
    public AudioClip introSound;
    public AudioClip createSound;
    public AudioClip destroySound;
    public AudioClip closeSound;
    public bool boolChanged = false;
    public bool boolChanged2 = false;

    // Enum for window sizes
    enum WindowSizeEnum
    {
        Tiny,
        Small,
        Medium,
        Imposant,
        Large,
        Huge
    };
    // Current window size enum selected
    WindowSizeEnum currentWindowSize;

    // List of asset objects
    List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

    // Adds access to the window through the toolbar
    [MenuItem("Tools/Resource Generator")]

    ///<summary>
    ///Function being called when opening the window
    ///</summary>
    public static void ShowWindow()
    {
        ResourceGenerator window = (ResourceGenerator)GetWindow(typeof(ResourceGenerator)); // Sets the title of the window
        window.minSize = new Vector2(minX, minY); // Minimal window size
        window.maxSize = new Vector2(minX, minY); // Maximal window size
    }

    ///<summary>
    ///Function called when the editor window first opens
    ///</summary>
    private void OnEnable()
    {             
        // Initialising variables
        animatedValue = new AnimBool(false);
        animatedValue.valueChanged.AddListener(Repaint);
        animatedValue2 = new AnimBool(false);
        animatedValue2.valueChanged.AddListener(Repaint);
        animatedValue3 = new AnimBool(false);
        animatedValue3.valueChanged.AddListener(Repaint);
        generatedList = new GenerateList();
        global = this;

        CreateFolders();       
        TryGetUnityObjectsOfTypeFromPath(ReturnDirectPath(), assets);

        // Initialising variables
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

        // Plays opening sound
        PlayClip(introSound);
    }

    ///<summary>
    ///Window Editing
    ///</summary>
    private void OnGUI()
    {
        SetColors();
        DrawTitles();
        TerrainRaycast();
        PlaceButtons();
        CheckSelected();
        ChangeWindowSize();
        PlaySounds();
        ParametersAndGeneration();
        BoxReshaping();
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

    ///<summary>
    ///When the window is closed
    ///</summary>
    private void OnDisable()
    {
        DestroyImmediate(editorBox); // Removes the spawn area
        PlayClip(closeSound); // Plays closing sound
    }

    ///<summary>
    ///Calls the create button function for each resource, which creates a button with an image
    ///</summary>
    void PlaceButtons()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // Creates a button for each resource
        foreach (var asset in assets)
        {
            CreateButton(asset.name);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // Toggle box for terrain parameters
        animatedValue2.target = EditorGUILayout.ToggleLeft("Terrain?", animatedValue2.target, skin.GetStyle("Sexy3"));

        // Terrain parameters appear if the toggle is on
        if (EditorGUILayout.BeginFadeGroup(animatedValue2.faded))
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select terrains", skin.GetStyle("Sexy2"));
            EditorGUILayout.EndHorizontal();

            SetTerrainTextures();

            GUILayout.Space(10);
        }
        EditorGUILayout.EndFadeGroup();

        animatedValue3.target = EditorGUILayout.ToggleLeft("Area of denial?", animatedValue3.target, skin.GetStyle("Sexy3"));

        if (EditorGUILayout.BeginFadeGroup(animatedValue3.faded))
        {  // Define the list of options for the dropdown

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select a texture", skin.GetStyle("Sexy2"));
            EditorGUILayout.EndHorizontal();
            SetTerrainTextures(true);

            GUILayout.Space(15);
      
            generatedList.AreaOfDenialRadius = EditorGUILayout.Slider("Area of denial radius", generatedList.AreaOfDenialRadius, 1f, 5.0f);
        }
        else
        {
            generatedList.TerrainTextureDenial = -1;
        }

        EditorGUILayout.EndFadeGroup();

        GUILayout.Space(10);
    }

    ///<summary>
    ///Creates buttons for the different terrain texture
    ///</summary>
    void SetTerrainTextures(bool denialBool = false)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Terrain terrain = Terrain.activeTerrain;
        for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
        {
            CreateButton(terrain.terrainData.terrainLayers[i].diffuseTexture.name, terrain.terrainData.terrainLayers[i].diffuseTexture, denialBool);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    ///<summary>
    ///Creates the input fields, checks validity of input, generates resources
    ///</summary>
    void ParametersAndGeneration()
    {
        if (EditorGUILayout.BeginFadeGroup(animatedValue.faded))
        {
            EditorGUILayout.BeginVertical("box");

            generatedList.resourcePrefab = newResourcePrefab;

            generatedList.numberOfResources = EditorGUILayout.IntField("Number of Resources", generatedList.numberOfResources);

            generatedList.rangeWidth = EditorGUILayout.FloatField("Spawn area X", generatedList.rangeWidth);

            generatedList.rangeHeight = EditorGUILayout.FloatField("Spawn area Z", generatedList.rangeHeight);

            generatedList.positionOnTerrain = EditorGUILayout.Vector2Field("Position on terrain", generatedList.positionOnTerrain);

            generatedList.minDistance = EditorGUILayout.FloatField("Distance between objects", generatedList.minDistance);

            generatedList.minY = EditorGUILayout.FloatField("Lowest spawn height", generatedList.minY);

            if (generatedList.minY < 0)
            {
                EditorGUILayout.TextArea("Below zero is sea level!", ReturnGUIStyle(15, color: Color.red));
            }


            generatedList.maxY = EditorGUILayout.FloatField("Highest spawn height", generatedList.maxY);

            if (GUILayout.Button("Generate", ReturnGUIStyle(30, "button")))
            {
                generationSucessful = generatedList.GenerateResources();
                PlayClip(createSound);
            }

            if (!generationSucessful)
            {
                EditorGUILayout.TextArea("No suitable area to generate with the values you have given!", ReturnGUIStyle(15, color: Color.red));
            }

            if (GUILayout.Button("Delete", ReturnGUIStyle(30, "button")))
            {
                generatedList.ClearResourceList();
                PlayClip(destroySound);
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndFadeGroup();


        GUIStyle customButtonStyle = new GUIStyle();
        customButtonStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("Generate Height Based Textures", ReturnGUIStyle(30, "button")))
        {
            GenerateTerrainTextures();
        }

    }

    ///<summary>
    ///Returns the height of the terrain at position
    ///</summary>
    private float GetTerrainHeight(Vector3 position)
    {
        float height = Terrain.activeTerrain.SampleHeight(position);

        // Adjust height based on terrain position and size
        height = height * Terrain.activeTerrain.terrainData.size.y / Terrain.activeTerrain.terrainData.heightmapResolution;

        return height;
    }

    ///<summary>
    ///Generates the terrain texture based on its height
    ///</summary>
    void GenerateTerrainTextures()
    {
        int callsInt = 0;

        generatedList.AreaOfDenialRadius = 10;

        for (int x = 0; x < Terrain.activeTerrain.terrainData.size.x; x += (int)generatedList.AreaOfDenialRadius)
        {
            for (int z = 0; z < Terrain.activeTerrain.terrainData.size.z; z += (int)generatedList.AreaOfDenialRadius)
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

                generatedList.TerrainTextureDenial = 0;

                if (GetTerrainHeight(worldPosition) > 5)
                {
                    generatedList.TerrainTextureDenial = 1;

                }
                if (GetTerrainHeight(worldPosition) > 20)
                {
                    generatedList.TerrainTextureDenial = 2;
                }

                if (GetTerrainHeight(worldPosition) > 30) //snow
                {
                    generatedList.TerrainTextureDenial = 3;
                }

                generatedList.ChangeTerrainTexture(worldPosition);

            }
        }

        generatedList.AreaOfDenialRadius = 2;
        generatedList.TerrainTextureDenial = -1;
    }

    ///<summary>
    ///Creates and initialises texture
    ///</summary>
    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
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
    public void CreateButton(string prefabNameString, Texture texture = null, bool denialBool = false)
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

        bool selectedBool = false;
        int denialselected = 0;

        if (terrainBool)
        {
            if (denialBool)
            {
                for (int i = 0; i < Terrain.activeTerrain.terrainData.terrainLayers.Length; i++)
                {
                    if (Terrain.activeTerrain.terrainData.terrainLayers[i].diffuseTexture == texture)
                    {
                        denialselected = i;
                        selectedBool = generatedList.TerrainTextureDenial == i;
                        break;
                    }
                }
            }
            else
            {
                selectedBool = generatedList.SelectTexturesList.Contains(texture);
            }
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
                if (denialBool)
                {
                    generatedList.TerrainTextureDenial = selectedBool ? -1 : denialselected;
                    areaSelected = !selectedBool;
                    
                    StopAllClips();
                    PlayClip(click2Sound);
                }
                else
                {
                    if (selectedBool)
                    {
                        generatedList.SelectTexturesList.Remove(texture);
                        StopAllClips();
                        PlayClip(click2Sound);
                    }
                    else
                    {
                        generatedList.SelectTexturesList.Add(texture);
                        StopAllClips();
                        PlayClip(click2Sound);
                    }
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

    ///<summary>
    ///Checks which steps have been completed and what parameters should be displayed
    ///</summary>
    void CheckSelected()
    {
        // Checking if the tickbox is toggled. Could bypass this step but decided to use a more readable boolean
        if (animatedValue2.target == true)
        {
            terrainToggleSelected = true;
        }
        else
        {
            terrainToggleSelected = false;
        }

        // Checking if the tickbox is toggled.
        if (animatedValue3.target == true)
        {
            areaToggleSelected = true;
        }
        else
        {
            areaToggleSelected = false;
        }

        // If at least one texture button is selected then this boolean is true
        if (generatedList.SelectTexturesList.Count > 0)
        {
            terrainSelected = true;
        }
        else
        {
            terrainSelected = false;
        }

        // If the tickbox is toggled, it requires a texture to be also selected to display the parameters
        if (terrainToggleSelected || areaToggleSelected)
        {
            if (terrainToggleSelected && areaToggleSelected)
            {
                if (terrainSelected && areaSelected) // If texture is selected
                {
                    animatedValue.target = resourceSelected;
                }
                else
                {
                    animatedValue.target = false;
                }
            }
            else if (terrainToggleSelected)
            {
                if (terrainSelected) // If texture is selected
                {
                    animatedValue.target = resourceSelected;
                }
                else
                {
                    animatedValue.target = false;
                }
            }
            else if (areaToggleSelected)
            {
                if (areaSelected) // If texture is selected
                {
                    animatedValue.target = resourceSelected;
                }
                else
                {
                    animatedValue.target = false;
                }
            }
        }
        else // If the tickbox is not toggled or untoggled, display the parameters if a resource is toggled
        {
            animatedValue.target = resourceSelected; // Displays if resourceSelected is true
        }
        if (!terrainToggleSelected)
        {
            if (generatedList.SelectTexturesList.Count > 0) // If a texture was selected when the tickbox was untoggled
            {
                for (int i = 0; i < generatedList.SelectTexturesList.Count; i++) // Unselect all textures
                {
                    generatedList.SelectTexturesList.Remove(generatedList.SelectTexturesList[i]);
                }
            }
        }
    }

    ///<summary>
    ///Resizes the window based on what parameters are displayed
    ///</summary>
    void ChangeWindowSize()
    {
        if (animatedValue.target == false && terrainToggleSelected == false && areaToggleSelected == false)
        {
            currentWindowSize = WindowSizeEnum.Tiny;
        }
        else if (animatedValue.target == false && (terrainToggleSelected == true || areaToggleSelected == true) && !(terrainToggleSelected == true && areaToggleSelected == true))
        {
            currentWindowSize = WindowSizeEnum.Small;
        }
        else if (animatedValue.target == true && terrainToggleSelected == false && areaToggleSelected == false)
        {
            currentWindowSize = WindowSizeEnum.Medium;
        }
        else if (animatedValue.target == false && terrainToggleSelected == true && areaToggleSelected == true)
        {
            currentWindowSize = WindowSizeEnum.Imposant;
        }
        else if (animatedValue.target == true && (terrainToggleSelected == true || areaToggleSelected == true) && !(terrainToggleSelected == true && areaToggleSelected == true))
        {
            currentWindowSize = WindowSizeEnum.Large;
        }
        else if (animatedValue.target == true && terrainToggleSelected == true && areaToggleSelected == true)
        {
            currentWindowSize = WindowSizeEnum.Huge;
        }

        if (currentWindowSize == WindowSizeEnum.Tiny)
        {
            windowYsize = 281.0f;
        }
        else if (currentWindowSize == WindowSizeEnum.Small)
        {
            if (areaToggleSelected == true)
            {
                windowYsize = 450.0f;
            }
            else if (terrainToggleSelected == true)
            {
                windowYsize = 425.0f;
            }
        }
        else if (currentWindowSize == WindowSizeEnum.Medium)
        {
            windowYsize = 535.0f;
        }
        else if (currentWindowSize == WindowSizeEnum.Imposant)
        {
            windowYsize = 594.0f;
        }
        else if (currentWindowSize == WindowSizeEnum.Large)
        {
            if (areaToggleSelected == true)
            {
                windowYsize = 705.0f;
            }
            else if (terrainToggleSelected == true)
            {
                windowYsize = 680.0f;
            }
        }
        else if (currentWindowSize == WindowSizeEnum.Huge)
        {
            windowYsize = 848.0f;
        }

        if (Screen.height - 27.0f != windowYsize)
        {
            minSize = new Vector2(windowXsize, windowYsize);
            maxSize = new Vector2(windowXsize, windowYsize);
        }
    }

    ///<summary>
    ///Allows to play sound in the editor
    ///</summary>
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

    ///<summary>
    ///Stops all sounds from playing
    ///</summary>
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

    ///<summary>
    ///Plays sound when the tickboxes are toggled
    ///</summary>
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

    ///<summary>
    ///Gets the spawn area box from the resource folder and makes it appear into the scene 
    ///</summary>   
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

    ///<summary>
    ///Returns an array of all the resource prefabs from the scene folder in Resources
    ///</summary>
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

    ///<summary>
    ///Makes sure there is a terrain
    ///</summary>
    void TerrainRaycast()
    {
        if (!Terrain.activeTerrain)
        {
            GUILayout.Label("You need a terrain to begin!", ReturnGUIStyle(30, "", Color.red));
            return;
        }
        Terrain.activeTerrain.gameObject.layer = LayerMask.NameToLayer("Terrain");
    }

    ///<summary>
    ///Changes the spawn area visually based on input
    ///</summary>
    void BoxReshaping()
    {
        editorBox.transform.position = generatedList.CalculatePosition();
        editorBox.transform.position = new Vector3(editorBox.transform.position.x, generatedList.minY + ((generatedList.maxY - generatedList.minY) / 2), editorBox.transform.position.z);
        editorBox.transform.localScale = new Vector3(generatedList.rangeWidth, generatedList.maxY - generatedList.minY, generatedList.rangeHeight);
    }
}
#endif // The end of the unity editor checker