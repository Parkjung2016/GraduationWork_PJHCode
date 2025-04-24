using System;
using Cysharp.Threading.Tasks;
using RayFire;
using UnityEngine;

namespace Main.Runtime.Equipments.Scripts.Weapons.Core
{
    public class WeaponFracture : MonoBehaviour
    {
        [SerializeField] private RayfireRigidRoot _shatterGroup;
        [SerializeField] private RayfireBomb _bomb;
        private void Start()
        {
            _shatterGroup.gameObject.SetActive(false);
        }

        public async void Explode()
        {
            try
            {
                transform.SetParent(null);
                _shatterGroup.gameObject.SetActive(true);
                _bomb.Explode(0);
                await UniTask.WaitForSeconds(_shatterGroup.fading.fadeTime + _shatterGroup.fading.lifeTime + 4,
                    cancellationToken: this.GetCancellationTokenOnDestroy());

                Destroy(gameObject);
            }
            catch (Exception e)
            {
            }
        }
    }
}