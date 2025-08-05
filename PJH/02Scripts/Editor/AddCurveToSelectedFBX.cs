using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PJH.Editor
{
    public class AddCurveToSelectedFBX : UnityEditor.Editor
    {
        [MenuItem("Tools/Add Curve To Selected FBX Clips")]
        public static void AddCurveToSelected()
        {
            var selected = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

            foreach (var obj in selected)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null || !importer.importAnimation)
                    continue;

                // AnimationClip 설정 복사 후 수정
                var clipAnimations = new List<ModelImporterClipAnimation>(importer.clipAnimations);

                for (int i = 0; i < clipAnimations.Count; i++)
                {
                    var clip = clipAnimations[i];
                    var curveBindings = new List<ClipAnimationInfoCurve>();

                    // 예시: 0초부터 끝까지 1.0값으로 유지되는 커브
                    AnimationCurve customCurve = AnimationCurve.Constant(0, 1, 1.0f);

                    // Animator float 파라미터로 인식되게 추가
                    curveBindings.Add(new ClipAnimationInfoCurve
                    {
                        name = "RootMotionMultiplier",
                        curve = customCurve
                    });

                    clip.curves = curveBindings.ToArray();
                    clipAnimations[i] = clip;
                }

                importer.clipAnimations = clipAnimations.ToArray();
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                Debug.Log($"✅ {path} - 커브 추가 완료!");
            }
        }
    }
}