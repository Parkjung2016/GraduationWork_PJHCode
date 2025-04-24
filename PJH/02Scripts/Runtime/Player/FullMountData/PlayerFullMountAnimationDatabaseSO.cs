using System;
using System.Collections.Generic;
using Animancer;
using Main.Runtime.Equipments.Datas;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Runtime.Players
{
    [CreateAssetMenu(menuName = "SO/Player/FullMount/FullMountAnimationDatabase")]
    public class PlayerFullMountAnimationDatabaseSO : SerializedScriptableObject
    {
        public List<FullMountAnimationDataSO> fullMountAnimationDats = new();
    }
}