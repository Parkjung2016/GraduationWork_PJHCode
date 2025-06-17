#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

public class BatchFbxRootTransformFixer : EditorWindow
{
    [MenuItem("Tools/Batch Fix FBX Root Transform")]
    static void FixRootTransformSettings()
    {
        string[] guids = Selection.assetGUIDs;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileName(path).Replace(".FBX", "");
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null && importer.animationType != ModelImporterAnimationType.None)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.importAnimation = true;

                ModelImporterClipAnimation[] clips = importer.clipAnimations;
                if (clips.Length == 0)
                {
                    clips = importer.defaultClipAnimations;
                }

                foreach (var clip in clips)
                {
                    clip.name = fileName;
                    clip.lockRootRotation = true;
                    clip.keepOriginalOrientation = true;
                    clip.heightFromFeet = true;
                    clip.lockRootHeightY = true;
                    clip.lockRootPositionXZ = false;
                }

                importer.clipAnimations = clips;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Updated Root Transform settings: {path}");
            }
        }

        AssetDatabase.Refresh();
    }
}
#endif