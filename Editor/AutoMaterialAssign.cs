using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoMaterialAssign : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

   //Material mat;
    string query;
    string exclude;
    int slot;
    bool searched;
    List<MeshRenderer> meshRenderers;
    //bool perfectMatch;
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Assign Material")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        AutoMaterialAssign window = (AutoMaterialAssign)EditorWindow.GetWindow(typeof(AutoMaterialAssign));
        window.Show();
    }
    Vector2 scroll;
    int length;
    List<Material> materials;
    bool folded;

    bool usePreset;
    AutoMaterialAssignSettings settings;
    void OnGUI()
    {
        if (materials == null)
        {
            materials = new List<Material>();
        }
        GUILayout.Label("Search Settings", EditorStyles.boldLabel);
        query = EditorGUILayout.TextField("Object name", query);
        exclude = EditorGUILayout.TextField("Exclude:", exclude);
        //query = EditorGUILayout.TextField("Object name", query);

        GUILayout.Label("Materials", EditorStyles.boldLabel);
        GUILayout.Label("Use Preset", EditorStyles.label);
        usePreset = EditorGUILayout.Toggle(usePreset);
        if (usePreset) {
            settings = (AutoMaterialAssignSettings)EditorGUILayout.ObjectField(settings,
             typeof(AutoMaterialAssignSettings),
             true);
        }
        else
        {
            GUILayout.Label("count", EditorStyles.label);
            length = EditorGUILayout.IntField(length);
            while (materials.Count < length)
            {
                materials.Add(null);
            }

            if (!folded)
            {
                if (GUILayout.Button("fold"))
                {
                    folded = true;
                }
                for (int i = 0; i < length; i++)
                {
                    materials[i] = (Material)EditorGUILayout.ObjectField(materials[i], typeof(Material), true);
                }
            }
            else
            {
                if (GUILayout.Button("unfold"))
                {
                    folded = false;
                }
            }
        }
        //mat = (Material)EditorGUILayout.ObjectField(mat, typeof(Material), true);

        GUILayout.Label("slot", EditorStyles.label);
        slot = EditorGUILayout.IntField(slot);

        if (GUILayout.Button("Search"))
        {
            SearchObject();
        }
        if (searched && meshRenderers != null)
        {
            if (GUILayout.Button("Assign"))
            {
                ReplaceMaterial();
            }
            EditorGUILayout.BeginScrollView(scroll);
            GUILayout.Label("Search Results", EditorStyles.boldLabel);
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                GUILayout.Label(meshRenderers[i].name, EditorStyles.label);
            }
            EditorGUILayout.EndScrollView();
        }
    }
    void SearchObject()
    {
        searched = true;
        Transform[] objs = (Transform[])FindObjectsOfType(typeof(Transform));
        meshRenderers = new List<MeshRenderer>();
        Debug.Log(objs.Length);
        for (int i = 0; i < objs.Length; i++)
        {
            string n = objs[i].name;
            if (n.Length >= query.Length && n.Substring(0, query.Length) == query &&
                (exclude == null || exclude == "" || n.Length < exclude.Length || n.Substring(0, exclude.Length) != exclude))
            {
                MeshRenderer mr = objs[i].GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    meshRenderers.Add(mr);
                }
            }
        }
    }
    void ReplaceMaterial()
    {
        string str = "";
        if (slot < 0)
        {
            Debug.Log("slot should be greater than or equal to zero");
            return;
        }
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            if (mr.sharedMaterials.Length <= slot)
            {
                str += mr.name + ",";
                continue;
            }
            if (!usePreset)
            {
                mr.sharedMaterials = GenerateReplacedArray(mr.sharedMaterials, slot, materials[Random.Range(0, length)]);
            }
            else
            {
                mr.sharedMaterials = GenerateReplacedArray(mr.sharedMaterials, settings.slot, settings.materials[Random.Range(0, settings.materials.Count)]);
            }
            //mr.material = mat;
        }
        //Debug.Log(str);
    }
    Material[] GenerateReplacedArray(Material[] array,int id,Material item)
    {
        Material[] res = new Material[array.Length];
        for(int i = 0; i < array.Length; i++)
        {
            if (i == id)
            {
                res[i] = item;
            }
            else
            {
                res[i] = array[i];
            }
        }
        return res;
    }
}
