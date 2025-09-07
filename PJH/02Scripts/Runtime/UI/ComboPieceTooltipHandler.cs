using System;
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
            _uiEventChannel.AddListener<ShowComboPieceTooltipUI>(HandleShowComboPieceTooltipUI);
            
        }

        private void OnDestroy()
        {
            DisposeHoverCancellationTokenSource();
            _uiEventChannel.RemoveListener<ShowComboPieceTooltipUI>(HandleShowComboPieceTooltipUI);
        }

        public void SetComboPieceSO(CommandActionPieceSO comboSO)
        {
            _comboSO = comboSO;
        }

        private void DisposeHoverCancellationTokenSource()
        {
            if (_hoverCancellationTokenSource is { IsCancellationRequested: false })
            {
                _hoverCancellationTokenSource.Cancel();
                _hoverCancellationTokenSource.Dispose();
            }
        }

        private void HandleShowComboPieceTooltipUI(ShowComboPieceTooltipUI evt)
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

                var showComboPiecePreviewUIEvt = UIEvent.ShowComboPieceTooltipUI;
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

                var showComboPieceTooltipUIEvt = UIEvent.ShowComboPieceTooltipUI;
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
            var currentPreviewEvt = UIEvent.ShowComboPieceTooltipUI;

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

        public void OnPointerExit(PointerEventData eventData)
        {
            DisposeHoverCancellationTokenSource();
            _hoverCancellationTokenSource = new CancellationTokenSource();
            HideTooltipAfterDelay(_hoverCancellationTokenSource.Token).Forget();
        }
    }
}