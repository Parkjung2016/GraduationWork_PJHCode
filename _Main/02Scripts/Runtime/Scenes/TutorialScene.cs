using Main.Runtime.Core.Events;
using Main.Scenes;


namespace Main.Scenes
{
    public class TutorialScene : BattleScene
    {
        protected override void Awake()
        {
            _autoPlayBGM = false;
            base.Awake();
            _gameEventChannel.AddListener<StartTutorial>(HandleStartTutorial);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _gameEventChannel.RemoveListener<StartTutorial>(HandleStartTutorial);
        }

        private void HandleStartTutorial(StartTutorial evt)
        {
            PlayBGM();
        }
    }
}