using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using Opsive.GraphDesigner.Runtime.Variables;
using PJH.Runtime.BossSkill.BossSkills.ShadowClones;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = Main.Core.Debug;

namespace Main.Runtime.Characters.StateMachine
{
    public class ShadowCloneStateSystem : SerializedMonoBehaviour, IAgentComponent
    {
        public SharedVariable<ShadowCloneStateSO> CurrentState { get; private set; }
        [SerializeField, InlineEditor] private ShadowCloneStateListSO shadowCloneStateList;

        [SerializeField, DictionaryDrawerSettings(KeyLabel = "CurrentState", ValueLabel = "BlackStates")]
        private Dictionary<ShadowCloneStateSO, List<ShadowCloneStateSO>> _transitionBlacklist;

        private ShadowClone _shadowClone;
        private Dictionary<string, ShadowCloneStateSO> _stateDictionary;


        public async void Initialize(Agent agent)
        {
            _shadowClone = agent as ShadowClone;
            await UniTask.WaitUntil(() => _shadowClone.BT,
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            CurrentState =
                _shadowClone.BT.GetVariable<ShadowCloneStateSO>("CurrentState");
            _stateDictionary = new();
            shadowCloneStateList.states.ForEach(state =>
            {
                string stateName = state.name.Replace("SO", "");
                _stateDictionary.Add(stateName, state);
            });
        }

        public ShadowCloneStateSO GetState(string stateName)
        {
            if (_stateDictionary.TryGetValue(stateName, out ShadowCloneStateSO state))
            {
                return state;
            }

            Debug.LogError($"{stateName} not found in ShadowCloneStateSystem.");
            return null;
        }

        public void ChangeState(string stateName)
        {
            ShadowCloneStateSO nextState = GetState(stateName);
            ChangeState(nextState);
        }

        public async void ChangeState(ShadowCloneStateSO nextState, bool isForceTransition = false)
        {
            if (nextState)
            {
                if (CurrentState.Value && !isForceTransition && _transitionBlacklist.ContainsKey(CurrentState.Value) &&
                    _transitionBlacklist[CurrentState.Value].Contains(nextState)) return;
                CurrentState.Value = null;
                await UniTask.DelayFrame(2, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                CurrentState.Value = nextState;
            }
            else
            {
                CurrentState.Value = null;
            }
        }
    }
}