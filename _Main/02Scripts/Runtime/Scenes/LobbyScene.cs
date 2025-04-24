using UnityEngine;

namespace Main.Scenes
{
    public class LobbyScene : BaseScene
    {
        protected override void Start()
        {
            base.Start();
            Application.targetFrameRate = 60;
        }
    }
}