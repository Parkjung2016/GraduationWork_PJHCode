using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NGS.MeshFusionPro;
using NGS.AdvancedCullingSystem.Dynamic;

namespace NGS.Compatibility
{
    [RequireComponent(typeof(MeshFusionSource))]
    public class DC_MFP_Compatibility : MonoBehaviour
    {
        private static HashSet<GameObject> _cullingObjects = new HashSet<GameObject>();

        [SerializeField] private int _cullingControllerID;

        [SerializeField] private CullingMethod _cullingMethod;

        private void Awake()
        {
            GetComponent<MeshFusionSource>().onCombineFinished += OnCombineFinished;
        }

        private void OnCombineFinished(MeshFusionSource source, IEnumerable<ICombinedObjectPart> parts)
        {
            foreach (var part in parts)
            {
                GameObject rootGO = ((MonoBehaviour)part.Root).gameObject;

                if (_cullingObjects.Contains(rootGO))
                    return;

                DC_SourceSettings settings = rootGO.AddComponent<DC_SourceSettings>();

                settings.ControllerID = _cullingControllerID;

                if (part.Root is CombinedLODGroup)
                {
                    settings.SourceType = SourceType.LODGroup;
                    settings
                        .GetStrategy<DC_LODGroupSourceSettingsStrategy>()
                        .CullingMethod = _cullingMethod;
                }
                else
                {
                    settings
                        .GetStrategy<DC_RendererSourceSettingsStrategy>()
                        .CullingMethod = _cullingMethod;
                }

                _cullingObjects.Add(rootGO);
            }
        }
    }
}