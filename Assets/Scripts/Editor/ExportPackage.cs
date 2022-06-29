using UnityEngine;
using UnityEditor;
 
/// <summary>
/// Script responsible for, when exporting the project to the .unitypackage, to include with it all the project info in regards of its Tags, Inputs, Settings and its Physics2D 
/// configuration (mainly for the collision matrix)
/// </summary>
public static class ExportPackage {
 

    [MenuItem("Export/Export with tags, layers, input settings and physics 2D")]
    public static void export()
    {
        string[] projectContent = new string[] {"Assets", "ProjectSettings/TagManager.asset","ProjectSettings/InputManager.asset",
                                                "ProjectSettings/ProjectSettings.asset", "ProjectSettings/Physics2DSettings.asset"};
        AssetDatabase.ExportPackage(projectContent, "Done.unitypackage",ExportPackageOptions.Interactive | ExportPackageOptions.Recurse |ExportPackageOptions.IncludeDependencies);
        Debug.Log("Project Exported");
    }
 
}