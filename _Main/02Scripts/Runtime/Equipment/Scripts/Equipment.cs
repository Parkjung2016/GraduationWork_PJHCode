using System;
using Main.Runtime.Combat;
using Main.Runtime.Equipments.Datas;
using Main.Shared;
using UnityEngine;

namespace Main.Runtime.Equipments.Scripts
{
    public abstract class Equipment : MonoBehaviour
    {
        [SerializeField] private EquipmentDataSO _equipmentData;

        public EquipmentDataSO EquipmentData => _equipmentData;
        protected IAgent _owner;

        protected virtual void Awake()
        {
        }

        public virtual void Equip(IAgent owner)
        {
            _owner = owner;
        }

        public virtual void UnEquip()
        {
            _owner = null;
        }
    }
}