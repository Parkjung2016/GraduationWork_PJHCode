using UnityEngine;
using UnityEngine.VFX;
using ZLinq;

namespace Main.Runtime.Core
{
    public class DistanceFade : MonoBehaviour
    {
        private readonly int FadeAmountHash = Shader.PropertyToID("_FadeAmount");
        [SerializeField] private Vector2 _minMaxDistance;
        [SerializeField] private bool _isRootObject = true;
        private Renderer[] _renderers;

        private Collider _collider;

        public bool Locked { get; set; }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!GetComponent<Collider>())
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }
#endif

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            Transform parentTrm = transform.parent;
            if (_isRootObject)
                parentTrm = transform;
            _renderers = parentTrm.GetComponentsInChildren<Renderer>()
                .AsValueEnumerable().Where(x => x is not VFXRenderer).Where(x => x.material.HasFloat(FadeAmountHash))
                .ToArray();
        }

        private void FixedUpdate()
        {
            float value = 0;
            if (!Locked)
            {
                Transform mainCameraTrm = Camera.main.transform;

                Vector3 closestPoint = _collider.ClosestPoint(mainCameraTrm.transform.position);
                value = Vector3.Distance(closestPoint, mainCameraTrm.transform.position);

                value = Remap(value, _minMaxDistance.y, 0, _minMaxDistance.x, 1);
            }

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material
                    .SetFloat(FadeAmountHash, value);
            }
        }

        public void SetFadeAmount(float value)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material
                    .SetFloat(FadeAmountHash, value);
            }
        }

        private float Remap(float val, float in1, float in2, float out1, float out2)
        {
            return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
        }
    }
}