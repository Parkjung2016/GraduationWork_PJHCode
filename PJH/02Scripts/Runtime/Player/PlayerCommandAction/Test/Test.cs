using System.Collections.Generic;
using Main.Runtime.Agents;
using Main.Runtime.Core;
using PJH.Runtime.PlayerPassive;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJH.Runtime.Players
{
    public class Test : MonoBehaviour, IAgentComponent
    {
        [SerializeField] private CommandActionPieceSO[] _actionPiece;
        [SerializeField] private PassiveSO[] _passives;
        private PlayerCommandActionManager _commandActionManagerCompo;
        private Player _player;

        [ReadOnly] public bool startActionInput;
        [ReadOnly] public bool startCombineInput;
        [ReadOnly] public bool startAssignmentKey;

        [ReadOnly] public List<CommandActionPieceSO> pieceList = new();
        private CommandActionData actionData;

        public void Initialize(Agent agent)
        {
            for (int i = 0; i < _actionPiece.Length; i++)
            {
                _actionPiece[i] = Instantiate(_actionPiece[i]);
                if (_passives.Length >= i + 1 && _passives[i] != null)
                    _actionPiece[i].TryAddPassive(Instantiate(_passives[i]));
            }

            _player = agent as Player;
            _commandActionManagerCompo = _player.GetCompo<PlayerCommandActionManager>();
        }

        private void Update()
        {
            if (startActionInput)
            {
                if (Keyboard.current.digit5Key.wasPressedThisFrame)
                {
                    AddPiece(_actionPiece[0]);
                }

                if (Keyboard.current.digit6Key.wasPressedThisFrame)
                {
                    AddPiece(_actionPiece[1]);
                }

                if (Keyboard.current.digit7Key.wasPressedThisFrame)
                {
                    AddPiece(_actionPiece[2]);
                }
            }

            if (startAssignmentKey)
            {
                if (Keyboard.current.hKey.wasPressedThisFrame)
                {
                    AssignmentKey(0);
                }

                if (Keyboard.current.jKey.wasPressedThisFrame)
                {
                    AssignmentKey(1);
                }

                if (Keyboard.current.kKey.wasPressedThisFrame)
                {
                    AssignmentKey(2);
                }
            }

            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                if (!startActionInput)
                {
                    actionData = new();

                    Debug.Log("<color=red>행동 입력 시작</color>");
                    Debug.Log("<color=green>5 ~ 7번 키로 조각을 추가해라.</color>");
                    startActionInput = true;
                }
            }

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                if (startActionInput)
                {
                    startActionInput = false;
                    Debug.Log("<color=red>행동 입력 끝</color>");
                    Debug.Log("<color=green>할당 키를 정해라. H(1) J(2) K(3)</color>");
                    startAssignmentKey = true;
                }
            }

            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                if (!startCombineInput)
                {
                    Debug.Log("<color=red>조각 병합 시작</color>");
                    Debug.Log("<color=green>5 ~ 7번 키로 병합할 조각을 선택해라.</color>");
                    startCombineInput = true;
                }
            }

            if (startCombineInput)
            {
                if (Keyboard.current.digit5Key.wasPressedThisFrame)
                {
                    _player.GetCompo<PlayerCommandActionManager>().CommandActions[0].ExecuteCommandActionPieces[0]
                        .TryCombineCommandActionPiece(_actionPiece[0]);
                    startCombineInput = false;
                }

                if (Keyboard.current.digit6Key.wasPressedThisFrame)
                {
                    _player.GetCompo<PlayerCommandActionManager>().CommandActions[0].ExecuteCommandActionPieces[0]
                        .TryCombineCommandActionPiece(_actionPiece[1]);
                    startCombineInput = false;
                }

                if (Keyboard.current.digit7Key.wasPressedThisFrame)
                {
                    _player.GetCompo<PlayerCommandActionManager>().CommandActions[0].ExecuteCommandActionPieces[0]
                        .TryCombineCommandActionPiece(_actionPiece[2]);
                    startCombineInput = false;
                }
            }
        }

        void AssignmentKey(int key)
        {
            startAssignmentKey = false;
            _commandActionManagerCompo.ChangeCommandAction(key, actionData);
            Debug.Log("<color=yellow>완성</color>");
        }

        void AddPiece(CommandActionPieceSO piece)
        {
            if (actionData.TryAddCommandActionPiece(piece))
            {
                pieceList.Add(piece);
                print(piece);
            }
            else
                Debug.LogError("제한 갯수 넘음");
        }
    }
}