using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

[System.Serializable]
/// <summary>
/// Color panel data class.
/// </summary>
public class ColorPanelData
{
    /// <summary>
    /// Max colors.
    /// </summary>
    private int kMaxColors = 15;

    /// <summary>
    /// The number of colors.
    /// </summary>
    private int totColors = 1;

    /// <summary>
    /// The colors array.
    /// </summary>
    public Color[] colors;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorPanelData"/> class.
    /// </summary>
    public ColorPanelData()
    {
        setTotColors(1);
        colors = new Color[totColors];
    }

    /// <summary>
    /// Sets the total colors.
    /// </summary>
    /// <param name="colors">Colors.</param>
    public void setTotColors(int colors)
    {
        totColors = colors;
        if (colors > kMaxColors)
            totColors = kMaxColors;
    }

    /// <summary>
    /// Gets the total colors.
    /// </summary>
    /// <returns>The tot colors.</returns>
    public int getTotColors()
    {
        return totColors;
    }
}

/// <summary>
/// Manage the color array.
/// </summary>
public class ArrayManager
{
    /// <summary>
    /// Adds the colors to array.
    /// </summary>
    /// <param name="colorCount">Color count.</param>
    /// <param name="colorData">Color data.</param>
    public void addColors(int colorCount, ref ColorPanelData colorData)
    {
        Color[] storedColor = colorData.colors;

        // init an empty array
        if (storedColor == null)
        {
            colorData.colors = new Color[colorCount];
        }
        // fill array with stored data
        else
        {
            colorData.colors = new Color[colorCount];
            for (int i = 0; i < colorData.getTotColors(); ++i)
            {
                colorData.colors[i].r = storedColor[i].r;
                colorData.colors[i].g = storedColor[i].g;
                colorData.colors[i].b = storedColor[i].b;
                colorData.colors[i].a = storedColor[i].a;
            }
        }

        colorData.setTotColors(colorCount);
    }

    /// <summary>
    /// Removes the colors from array.
    /// </summary>
    /// <param name="colorCount">Color count.</param>
    /// <param name="colorData">Color data.</param>
    public void removeColors(int colorCount, ref ColorPanelData colorData)
    {
        Color[] storedColor = colorData.colors;
        colorData.colors = new Color[colorCount];
        
        for (int i = 0; i < colorCount; ++i)
        {
            colorData.colors[i].r = storedColor[i].r;
            colorData.colors[i].g = storedColor[i].g;
            colorData.colors[i].b = storedColor[i].b;
            colorData.colors[i].a = storedColor[i].a;
        }
        
        colorData.setTotColors(colorCount);
    }
}

/// <summary>
/// Color memo editor plugin main class.
/// </summary>
public class ColorMemo : EditorWindow
{
    /// <summary>
    /// The data file.
    /// </summary>
    private string kStoragePath = "Assets/Resources/";
    private string kStorageFile = "Assets/Resources/_color-data.dat";

    ArrayManager arrayManager = new ArrayManager();

    [MenuItem("Window/Color Palette")]
    static void Init()
    {
        GetWindow(typeof(ColorMemo));
    }

    void OnGUI()
    {
        GUI.skin.label.wordWrap = true;

        //
        // Load stored data, if exist.
        //
        ColorPanelData colorData = Load();

        separators(3);
        GUILayout.Label("Color palette");
        separators(1);
        line();
        separators(1);

        // Color number input layout
        EditorGUILayout.BeginHorizontal();
        {
            int colorCount = EditorGUILayout.IntField("Number of colors (max 15) : ", colorData.getTotColors());

            // skip if the input field is empty
            if (colorCount <= 0)
                return;

            // add new item
            if (colorCount > colorData.getTotColors())
            {
                arrayManager.addColors(colorCount, ref colorData);
                Save(colorData);
            }
            // remove items
            else if (colorCount < colorData.getTotColors())
            {
                arrayManager.removeColors(colorCount, ref colorData);
                Save(colorData);
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset colors"))
        {
            // all blacks!
            colorData.colors = new Color[colorData.getTotColors()];
            Save(colorData);
        }

        separators(1);
        line();
        separators(1);
        GUILayout.Label("Your colors are showed here. You can use it as you prefer or select a target and click apply to change its color.");
        separators(2);

        for (int i = 0; i < colorData.getTotColors(); ++i)
        {
            EditorGUILayout.BeginHorizontal();
            try
            {
                colorData.colors[i] = EditorGUILayout.ColorField("COLOR #" + (i + 1), colorData.colors[i]);
                if (GUILayout.Button("Apply"))
                    ChangeColors(colorData.colors[i]);
            }
            catch
            {

            }
            EditorGUILayout.EndHorizontal();
        }

        Save(colorData);

        separators(1);
        GUILayout.Label("Color are automatically stored in your \"Assets/Resource\" folder of current project.\nYou can export (or backup) simply copying/pasting the file.");

        GUI.enabled = true;
    }

    /// <summary>
    /// Apply the selected color to the selected target.
    /// </summary>
    /// <param name="color">Color.</param>
    private void ChangeColors(Color color)
    {
        // the active game object
        if (Selection.activeGameObject)
            foreach (GameObject t in Selection.gameObjects)
            {
                Renderer rend = t.GetComponent<Renderer>();
                if (rend != null)
                    rend.sharedMaterial.color = color;
            }
    }

    /// <summary>
    /// Load the color data from storage file.
    /// </summary>
    public ColorPanelData Load()
    {
        InitializeStorageFile();

        string data = File.ReadAllText(kStorageFile);

        // returns something empty
        if (data == null || data == "")
            return new ColorPanelData();

        /*
         * FILE FORMAT:
         * 
         * n
         * R:G:B:A
         * R:G:B:A
         * [...]
         * R:G:B:A
         * 
        */

        ColorPanelData colorData = new ColorPanelData();
        string[] cks = data.Split('\n');

        colorData.setTotColors(int.Parse(cks[0]));
        colorData.colors = new Color[colorData.getTotColors()];

        for (int i = 0; i < colorData.getTotColors(); ++i)
        {
            string[] colors = cks[i + 1].Split(':');

            colorData.colors[i].r = float.Parse(colors[0]);
            colorData.colors[i].g = float.Parse(colors[1]);
            colorData.colors[i].b = float.Parse(colors[2]);
            colorData.colors[i].a = float.Parse(colors[3]);
        }

        return colorData;
    }

    /// <summary>
    /// Save the color data to storage file.
    /// </summary>
    public void Save(ColorPanelData data)
    {
        InitializeStorageFile();

        // skip if nothing saved
        if (data == null || data.colors == null)
            return;

        string txtToWrite = data.getTotColors() + "\n";
        string colorsToAppend = string.Empty;
        for (int i = 0; i < data.colors.Length; ++i)
        {
            colorsToAppend = colorsToAppend + data.colors[i].r + ":" + data.colors[i].g + ":" + data.colors[i].b + ":" + data.colors[i].a + "\n";
        }

        txtToWrite = txtToWrite + colorsToAppend;

        /*
         * FILE FORMAT:
         * 
         * n
         * R:G:B:A
         * R:G:B:A
         * [...]
         * R:G:B:A
         * 
        */

        File.WriteAllText(kStorageFile, txtToWrite);
    }

    /// <summary>
    /// Initializes the storage file.
    /// </summary>
    void InitializeStorageFile()
    {
        // create or ignore "Resource" path
        Directory.CreateDirectory(kStoragePath);

        // create storage file if not exist
        FileInfo fi = new FileInfo(kStorageFile);
        if (!fi.Exists)
        {
            FileStream fs = fi.Create();
            fs.Close();
        }
    }

    /// <summary>
    /// Add separators...
    /// </summary>
    /// <param name="n">N.</param>
    private void separators(int n)
    {
        for (int i = 0; i < n; i++)
        {
            EditorGUILayout.Separator();
        }
    }

    /// <summary>
    /// Simulate a line separator. I like.
    /// </summary>
    private void line()
    {
        GUILayout.Box("", new GUILayoutOption[]{ GUILayout.ExpandWidth(true), GUILayout.Height(1) });
    }
}
