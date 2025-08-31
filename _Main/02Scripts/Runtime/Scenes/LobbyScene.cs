using PJH.Runtime.Core;
using UnityEngine;

namespace Main.Scenes
{
    public class LobbyScene : BaseScene
    {
        protected override void Start()
        {
            base.Start();
            BIS.Manager.Managers.Resource.Load<ComboSynthesisPriceInfoSO>("ComboSynthesisPriceInfo")
                .ResetIncreaseLevel();
            Application.targetFrameRate = 60;
            
        }
    }
}