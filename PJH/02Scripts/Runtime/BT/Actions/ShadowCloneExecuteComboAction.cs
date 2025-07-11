﻿using System.Collections.Generic;
using Main.Core;
using Main.Runtime.Agents;
using Main.Runtime.Combat;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using PJH.Runtime.BossSkill.BossSkills.ShadowClones;
using UnityEngine;

namespace PJH.Runtime.BT.Actions
{
    public class ShadowCloneExecuteComboAction : ShadowCloneActionNode
    {
        public SharedVariable<bool> _comboPossible;

        private ShadowCloneAnimator _animator;
        [SerializeField] private CombatDataListSO _combatDataList;

        private int _length;
        private int _curComboCnt;
        private CombatDataSO _combatData;

        private bool _animationEnd;
        private IReadOnlyList<CombatDataSO> _combatDatas;

        public override void OnAwake()
        {
            base.OnAwake();
            _animator = _shadowClone.GetCompo<ShadowCloneAnimator>();
        }

        public override void OnStart()
        {
            _animationEnd = false;
            _curComboCnt = 0;
            _combatData = null;
            _combatDatas = _combatDataList.combatDataList.GetRandom().combatDatas;
            _length = _combatDatas.Count;
            _comboPossible.Value = false;
            PlayCombatAnimation();
        }

        public override TaskStatus OnUpdate()
        {
            if (_comboPossible.Value)
            {
                if (_curComboCnt < _length)
                {
                    _comboPossible.Value = false;
                    PlayCombatAnimation();
                }
            }

            return _animationEnd ? TaskStatus.Success : TaskStatus.Running;
        }

        private void PlayCombatAnimation()
        {
            _combatData = _combatDatas[_curComboCnt++];
            _shadowClone.LookPlayer();
            _shadowClone.GetCompo<AgentWeaponManager>().CurrentCombatData = _combatData;
            _animator.PlayAnimationClip(_combatData.attackAnimationClip, () => { _animationEnd = true; });
            _shadowClone.GetCompo<ShadowCloneMovement>().MoveStop(false);
        }
    }
}