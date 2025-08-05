using Main.Runtime.Agents;
using Main.Runtime.Combat.Core;
using Main.Shared;
using UnityEngine;

namespace PJH.Trainingscarecrow
{
    public class TrainingScarecrow : Agent, ILockOnAble
    {
        [SerializeField] private Transform _headTrm;
        [SerializeField] private float _force = 100;
        private Rigidbody _rigidbodyCompo;
        public Transform LockOnUIDisplayTargetTrm { get; }

        protected override void Start()
        {
            base.Start();
            _rigidbodyCompo = GetComponent<Rigidbody>();
            HeadTrm = _headTrm;
        }

        protected override void HandleApplyDamaged(float damage)
        {
            GetDamagedInfo getDamagedInfo = HealthCompo.GetDamagedInfo;
            Vector3 pos = transform.position;
            pos.y = getDamagedInfo.hitPoint.y;
            Vector3 dir = (pos - getDamagedInfo.hitPoint).normalized;
            _rigidbodyCompo.AddForce(dir * _force, ForceMode.Impulse);
        }

    }
}