using Main.Runtime.Equipments.Scripts;
using UnityEngine;

namespace Main.Runtime.Equipments.Datas
{
    public abstract class EquipmentDataSO : ScriptableObject
    {
        public Equipment equipmentPrefab;
        public Vector3 equipmentPosition;
        public Quaternion equipmentRotation;
    }
}