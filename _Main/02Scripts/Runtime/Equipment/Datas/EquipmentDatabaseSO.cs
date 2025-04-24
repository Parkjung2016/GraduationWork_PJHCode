using System.Collections.Generic;
using Main.Runtime.Equipments.Scripts;
using UnityEngine;

namespace Main.Runtime.Equipments.Datas
{
    [CreateAssetMenu(menuName = "SO/Equipment/List")]
    public class EquipmentDatabaseSO : ScriptableObject
    {
        public List<EquipmentDataSO> equipmentDatas = new();
    }
}