using UnityEditor;
using UnityEngine;

public class SelectStartUpSceneLoader : Editor
{
    static readonly string menuPath = "씬 로더/플레이 시 타이틀 씬으로 이동";

    [MenuItem("씬 로더/플레이 시 타이틀 씬으로 이동")]
    private static void SceneLoader()
    {
        var checkPlag = GetChecked();
        Menu.SetChecked(menuPath, !checkPlag);
    }

    public static bool GetChecked() => Menu.GetChecked(menuPath);
}