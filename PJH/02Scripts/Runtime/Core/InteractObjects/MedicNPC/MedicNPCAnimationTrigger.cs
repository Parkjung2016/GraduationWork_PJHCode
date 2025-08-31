using Animancer;
using FMODUnity;
using Main.Runtime.Animators;
using UnityEngine;

namespace PJH.Runtime.Core.InteractObjects
{
    public class MedicNPCAnimationTrigger : MonoBehaviour
    {
        [SerializeField] private AnimancerEventAssetSO _destroyHealItemEvent, _playUseHealItemSoundEvent;
        [SerializeField] private GameObject _healItem;
        [SerializeField] private bool _destroyHealItem;
        [SerializeField] private EventReference _useHealItemSound;
        private HybridAnimancerComponent _hybridAnimancerCompo;

        private void Awake()
        {
            _hybridAnimancerCompo = GetComponent<HybridAnimancerComponent>();
            if (_destroyHealItemEvent != null)
                _hybridAnimancerCompo.Events.AddTo(_destroyHealItemEvent, HandleDestroyHealItem);
            if (_playUseHealItemSoundEvent != null)
                _hybridAnimancerCompo.Events.AddTo(_playUseHealItemSoundEvent, HandlePlayUseHealItemSoundEvent);
        }

        private void HandlePlayUseHealItemSoundEvent()
        {
            if (!_useHealItemSound.IsNull)
                RuntimeManager.PlayOneShot(_useHealItemSound, _healItem.transform.position);
        }

        private void HandleDestroyHealItem()
        {
            if (_destroyHealItem)
                _healItem?.gameObject.SetActive(false);
        }
    }
}