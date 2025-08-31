using Main.Runtime.Core.Events;


namespace Main.Scenes
{
    public class TutorialScene : BattleScene
    {
        private bool _startedBGM;

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
            if (_startedBGM) return;
            _startedBGM = true;
            PlayBGM();
        }
    }
}