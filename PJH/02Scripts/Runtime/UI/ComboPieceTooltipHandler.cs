using System.Threading;
using BIS.Events;
using Cysharp.Threading.Tasks;
using Main.Runtime.Core.Events;
using PJH.Runtime.Players;
using PJH.Utility.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PJH.Runtime.UI
{
    public class ComboPieceTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const float hoverTime = 0.5f;
        private GameEventChannelSO _uiEventChannel;
        private CancellationTokenSource _hoverCancellationTokenSource;

        private CommandActionPieceSO _comboSO;

        private void Awake()
        {
            _uiEventChannel = AddressableManager.Load<GameEventChannelSO>("UIEventChannelSO");
            _uiEventChannel.AddListener<ShowComboPieceTooltipUIEvent>(HandleShowComboPieceTooltipUI);
        }

        private void OnDestroy()
        {
            DisposeHoverCancellationTokenSource();
            _uiEventChannel.RemoveListener<ShowComboPieceTooltipUIEvent>(HandleShowComboPieceTooltipUI);
        }

        public void SetComboPieceSO(CommandActionPieceSO comboSO)
        {
            CommandActionPieceSO prevCombo = _comboSO;
            _comboSO = comboSO;
            if (_comboSO == null)
            {
                HideTooltip(prevCombo);
            }
        }

        private void DisposeHoverCancellationTokenSource()
        {
            if (_hoverCancellationTokenSource is { IsCancellationRequested: false })
            {
                _hoverCancellationTokenSource.Cancel();
                _hoverCancellationTokenSource.Dispose();
            }
        }

        private void HandleShowComboPieceTooltipUI(ShowComboPieceTooltipUIEvent evt)
        {
            if (evt.show)
            {
                DisposeHoverCancellationTokenSource();
            }
        }


        private async UniTaskVoid CheckHover(CancellationToken token)
        {
            try
            {
                await UniTask.WaitForSeconds(hoverTime, ignoreTimeScale: true, cancellationToken: token);

                var showComboPiecePreviewUIEvt = UIEvent.ShowComboPieceTooltipUIEvent;
                showComboPiecePreviewUIEvt.show = true;
                showComboPiecePreviewUIEvt.comboPiece = _comboSO;
                _uiEventChannel.RaiseEvent(showComboPiecePreviewUIEvt);
            }
            catch
            {
            }
        }

        private async UniTaskVoid HideTooltipAfterDelay(CancellationToken token)
        {
            try
            {
                await UniTask.WaitForSeconds(0.1f, ignoreTimeScale: true, cancellationToken: token);

                var showComboPieceTooltipUIEvt = UIEvent.ShowComboPieceTooltipUIEvent;
                if (showComboPieceTooltipUIEvt.show && showComboPieceTooltipUIEvt.comboPiece == _comboSO)
                {
                    showComboPieceTooltipUIEvt.show = false;
                    _uiEventChannel.RaiseEvent(showComboPieceTooltipUIEvt);
                }
            }
            catch
            {
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var currentPreviewEvt = UIEvent.ShowComboPieceTooltipUIEvent;

            if (currentPreviewEvt.show)
            {
                currentPreviewEvt.comboPiece = _comboSO;
                _uiEventChannel.RaiseEvent(currentPreviewEvt);
            }
            else
            {
                DisposeHoverCancellationTokenSource();

                _hoverCancellationTokenSource = new CancellationTokenSource();
                CheckHover(_hoverCancellationTokenSource.Token).Forget();
            }
        }

        private void HideTooltip(CommandActionPieceSO prevCombo)
        {
            DisposeHoverCancellationTokenSource();
            var showComboPieceTooltipUIEvt = UIEvent.ShowComboPieceTooltipUIEvent;
            if (showComboPieceTooltipUIEvt.show && showComboPieceTooltipUIEvt.comboPiece == prevCombo)
            {
                showComboPieceTooltipUIEvt.show = false;
                _uiEventChannel.RaiseEvent(showComboPieceTooltipUIEvt);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DisposeHoverCancellationTokenSource();
            _hoverCancellationTokenSource = new CancellationTokenSource();
            HideTooltipAfterDelay(_hoverCancellationTokenSource.Token).Forget();
        }
    }
}