using UnityEngine;

namespace Main.Shared
{
    public class Define
    {
        public enum ESocketType
        {
            LeftHand,
            RightHand,
            LeftLowerArm,
            RightLowerArm,
            LeftFoot,
            RightFoot,
            LeftLowerLeg,
            RightLowerLeg
        }
        public static class MLayerMask
        {
            public static readonly LayerMask WhatIsGround = LayerMask.GetMask("Ground");
            public static readonly LayerMask WhatIsEnemy = LayerMask.GetMask("Enemy");
            public static readonly LayerMask WhatIsPlayer = LayerMask.GetMask("Player");
        }
    }
}