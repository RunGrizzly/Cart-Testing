using UnityEditor;
using UnityEngine;

// Usage: Attach to gameobject, assign target gameobject (from where the mesh is taken), Run, Press savekey

public class SaveMeshInEditor : MonoBehaviour
{

    public KeyCode saveKey;
    public string saveName = "SavedMesh";
    public Transform selectedGameObject;

    private void Start()
    {
        Debug.Log("Initialised mesh saver");
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Saving Mesh");
            SaveAsset();
        }
    }

    void SaveAsset()
    {
        var mf = selectedGameObject.GetComponent<MeshFilter>();
        // if (mf)
        // {
        var savePath = "Assets/" + saveName + ".asset";
        Debug.Log("Saved Mesh to:" + savePath);
        AssetDatabase.CreateAsset(mf.mesh, savePath);
        //}
    }
}