using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using INab.Dissolve;
using UnityEngine;
using UnityEngine.VFX;

namespace PJH.Runtime.Core
{
    public class MotionTrail : MonoBehaviour, IPoolable
    {
        private readonly int meshVFXHash = Shader.PropertyToID("Mesh")
;
        private readonly int dissolveAmountVFXHash = Shader.PropertyToID("Dissolve Amount");
        private readonly int dissolveAmountShaderHash = Shader.PropertyToID("_DissolveAmount");
        [field: SerializeField] public PoolTypeSO PoolType { get; private set; }
        [SerializeField] private float _hideDuration = 1.5f;
        public GameObject GameObject => gameObject;
        private Pool _pool;

        private MeshFilter _meshFilter;
        private Mesh _mesh;
        private Dissolver _dissolver;
        private VisualEffect _dissolveEffect;

        public void SetUpPool(Pool pool)
        {
            _dissolver = GetComponent<Dissolver>();
            _dissolveEffect = GetComponentInChildren<VisualEffect>();
            _pool = pool;
            _mesh = new Mesh();
            _meshFilter = GetComponent<MeshFilter>();
            Material mat = GetComponent<MeshRenderer>().material;
            _dissolver.materials = new List<Material>() { mat };
        }

        public void ResetItem()
        {
            _dissolveEffect.SetFloat(dissolveAmountVFXHash, 0);
            _dissolver.materials[0].SetFloat(dissolveAmountShaderHash, 0);
        }

        public async void SnapshotMesh(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            try
            {
                skinnedMeshRenderer.BakeMesh(_mesh);
                _meshFilter.mesh = _mesh;
                _dissolveEffect.SetMesh(meshVFXHash, _mesh);
                transform.position = skinnedMeshRenderer.transform.position;
                transform.rotation = skinnedMeshRenderer.transform.rotation;

                await UniTask.WaitForSeconds(_hideDuration, cancellationToken: this.GetCancellationTokenOnDestroy());
                _dissolver.Dissolve();
                await UniTask.WaitForSeconds(_dissolver.duration + 4,
                    cancellationToken: this.GetCancellationTokenOnDestroy());
                _pool.Push(this);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}