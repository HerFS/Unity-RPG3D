using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
#endif

public class FbxExporter : MonoBehaviour
{
    [SerializeField]
    private GameObject[] exportObject;

#if UNITY_EDITOR
    [ContextMenu("CreateFBX")]
    private void CreateFBX()
    {
        if (exportObject == null)
        {
            return;
        }

        string path = Application.dataPath + "/Resources/ExportObject.fbx";
        ModelExporter.ExportObjects(path, exportObject);
    }
#endif
}
