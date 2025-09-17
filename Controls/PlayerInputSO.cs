using System;
using Main.Shared;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "SO/Player/InputSO")]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions, Controls.IInGameUIActions
    {
        public event Action<bool> RunEvent;
        public event Action EvadeEvent;
        public event Action<Vector3> MovementEvent;
        public event Action<bool> BlockEvent;
        public event Action<int> CommandActionEvent;
        public event Action AttackEvent;
        public event Action LockOnToggleEvent;
        public event Action ChangeLockOnTargetEvent;
        public event Action FullMountEvent;
        public event Action InteractEvent;
        public event Action FinisherEvent;
        public event Action ESCEvent;
        public event Action EnterEvent;
        public event Action TabbarEvent;
        public event Action MapOpenCloseEvent;
        public event Action ShopRerollEvent;
        public event Action<bool> SpaceEvent;
        public Vector3 Input { get; private set; }
        public Vector3 MouseDelta { get; private set; }

        [HideInInspector] public bool preventChangePlayerInput,
            preventAttackInput,
            preventBlockInput,
            preventChangeLockOnTargetEvent,
            preventInteractInput;

        private Controls _controls;
        public Controls Controls => _controls;

        public void ResetPreventVariable()
        {
            preventChangeLockOnTargetEvent = false;
            preventAttackInput = false;
            preventBlockInput = false;
            preventChangePlayerInput = false;
            preventInteractInput = false;
        }

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
                _controls.InGameUI.SetCallbacks(this);
            }

            EnablePlayerInput(true);
            EnableUIInput(true);
        }

        private void OnDisable()
        {
            EnablePlayerInput(false);
            EnableUIInput(false);
        }

        public void EnablePlayerInput(bool isEnabled)
        {
            if (preventChangePlayerInput) return;
            if (isEnabled)
            {
                _controls.Player.Enable();
            }
            else
            {
                _controls.Player.Disable();
            }
        }

        public void EnableUIInput(bool isEnabled)
        {
            if (isEnabled)
                _controls.InGameUI.Enable();
            else
                _controls.InGameUI.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            Input = new Vector3(input.x, 0, input.y);
            MovementEvent?.Invoke(Input);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            MouseDelta = context.ReadValue<Vector2>();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (preventInteractInput) return;
            if (context.performed)
                InteractEvent?.Invoke();
        }

        public void OnFinisher(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                FinisherEvent?.Invoke();
            }
        }

        public void OnAct2(InputAction.CallbackContext context)
        {
        }

        public void OnLockOn(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                LockOnToggleEvent?.Invoke();
            }
        }

        public void OnCommandAction1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CommandActionEvent?.Invoke(0);
            }
        }

        public void OnCommandAction2(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CommandActionEvent?.Invoke(1);
            }
        }

        public void OnCommandAction3(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CommandActionEvent?.Invoke(2);
            }
        }

        public void OnEvade(InputAction.CallbackContext context)
        {
            if (context.performed)
                EvadeEvent?.Invoke();
        }

        public void OnChangeLockOnTarget(InputAction.CallbackContext context)
        {
            if (preventChangeLockOnTargetEvent) return;
            if (context.performed)
            {
                ChangeLockOnTargetEvent?.Invoke();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (preventAttackInput) return;
            if (context.performed)
            {
                AttackEvent?.Invoke();
            }
        }

        public void OnAct1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (context.interaction is HoldInteraction)
                {
                    FullMountEvent?.Invoke();
                }
            }
        }


        public void OnBlock(InputAction.CallbackContext context)
        {
            if (!preventBlockInput && context.performed)
            {
                BlockEvent?.Invoke(true);
            }

            if (context.canceled)
                BlockEvent?.Invoke(false);
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RunEvent?.Invoke(true);
            }

            if (context.canceled)
                RunEvent?.Invoke(false);
        }

        public void OnESC(InputAction.CallbackContext context)
        {
            if (context.performed)
                ESCEvent?.Invoke();
        }

        public void OnEnter(InputAction.CallbackContext context)
        {
            if (context.performed)
                EnterEvent?.Invoke();
        }

        public void OnTab(InputAction.CallbackContext context)
        {
            if (context.performed)
                TabbarEvent?.Invoke();
        }

        public void OnSpace(InputAction.CallbackContext context)
        {
            if (context.performed)
                SpaceEvent?.Invoke(true);
            if (context.canceled)
                SpaceEvent?.Invoke(false);
        }

        public void OnM(InputAction.CallbackContext context)
        {
            if (context.performed)
                MapOpenCloseEvent?.Invoke();
        }

        public void OnShopReroll(InputAction.CallbackContext context)
        {
            if (context.performed)
                ShopRerollEvent?.Invoke();
        }
    }
}