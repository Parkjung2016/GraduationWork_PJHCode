using Opsive.BehaviorDesigner.Runtime.Tasks;
using PJH.Runtime.BossSkill.BossSkills.ShadowClones;
using UnityEngine;

namespace PJH.Runtime.BT.Actions
{
    public class ShadowCloneRoamingAroundPlayerAction : ShadowCloneActionNode
    {
        public float cirlceRadius = 5f;
        public float updateValueTime;
        public bool applyUpdateValueTimeVariance;
        public Vector2 updateValueTimeVariance;
        public bool applyRadiusVariance;
        public float circleRadiusVariance = 0f;
        private float _resultCircleRadius;
        private float _resultAngleSpeed;
        private float _angle = 0f;
        private float _nextUpdateTime;

        private ShadowCloneMovement _movementCompo;

        public override void OnAwake()
        {
            base.OnAwake();
            _movementCompo = _shadowClone.GetCompo<ShadowCloneMovement>();
        }

        public override void OnStart()
        {
            base.OnStart();
            _angle = 0;
            _movementCompo.SetCanMove(true);
            _movementCompo.SetRVOControllerLocked(false);
            UpdateValue();
        }

        public override TaskStatus OnUpdate()
        {
            _angle += _resultAngleSpeed * Time.deltaTime;
            float x = Mathf.Cos(_angle) * _resultCircleRadius;
            float z = Mathf.Sin(_angle) * _resultCircleRadius;

            Vector3 targetPosition = new Vector3(_player.transform.position.x + x, transform.position.y,
                _player.transform.position.z + z);

            _movementCompo.SetDestination(targetPosition);
            if (Time.time >= _nextUpdateTime)
            {
                UpdateValue();
            }

            return TaskStatus.Running;
        }

        private void UpdateValue()
        {
            _nextUpdateTime = Time.time + updateValueTime;
            if (applyUpdateValueTimeVariance)
                _nextUpdateTime += Random.Range(updateValueTimeVariance.x, updateValueTimeVariance.y);

            UpdateAngleSpeedSign();
            UpdateResultCircleRadius();
        }

        private void UpdateAngleSpeedSign()
        {
            int sign = Random.Range(0, 2) * 2 - 1;
            float angleSpeed = _movementCompo.AIPathCompo.maxSpeed * 0.2f;
            _resultAngleSpeed = angleSpeed * sign;
        }

        private void UpdateResultCircleRadius()
        {
            _resultCircleRadius = cirlceRadius;
            if (applyRadiusVariance)
                _resultCircleRadius += Random.Range(-circleRadiusVariance, circleRadiusVariance);
        }

        public override void OnEnd()
        {
            _movementCompo.SetCanMove(false);
        }
    }
}