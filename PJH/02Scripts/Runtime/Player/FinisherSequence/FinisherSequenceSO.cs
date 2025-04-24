using System.Collections.Generic;
using UnityEngine;

namespace PJH.Runtime.Players.FinisherSequence
{
    [CreateAssetMenu(menuName = "SO/Finisher/Sequence")]
    public class FinisherSequenceSO : ScriptableObject
    {
        public List<FinisherSequenceDataSO> sequenceDatas = new();
    }
}