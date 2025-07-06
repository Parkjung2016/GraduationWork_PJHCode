using System.Collections.Generic;
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