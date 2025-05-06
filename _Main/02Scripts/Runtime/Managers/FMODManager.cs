using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Main.Runtime.Manager
{
    public class FMODManager
    {
        private static string baseVCAPath = "vca:/";
        public static string GameSound_VCA_Path = baseVCAPath + "GameSound";
        public static string TimelineSound_VCA_Path = baseVCAPath + "TimelineSound";

        private static string baseBusPath = "bus:/";
        public static string Main_Bus_Path = baseBusPath + "Main";
        private static string Game_Bus_Path = Main_Bus_Path + "/Game";
        public static string SFX_Bus_Path = Game_Bus_Path + "/SFX";
        public static string Music_Bus_Path = Game_Bus_Path + "/Music";
        public static string UI_Bus_Path = Main_Bus_Path + "/UI";

        private VCA _subVCA, _timelineVCA;
        private Bus _sfxBus, _musicBus, _uiBus, _mainBus, _gameBus;


        private EventInstance _beforePlayerDeadSnapshot;
        public EventInstance MusicEventInstance { get; private set; }

        public void Init()
        {
            //_subVCA = RuntimeManager.GetVCA(GameSound_VCA_Path);
            //_timelineVCA = RuntimeManager.GetVCA(TimelineSound_VCA_Path);
            //_sfxBus = RuntimeManager.GetBus(SFX_Bus_Path);
            //_musicBus = RuntimeManager.GetBus(Music_Bus_Path);
            //_gameBus = RuntimeManager.GetBus(Game_Bus_Path);
            //_uiBus = RuntimeManager.GetBus(UI_Bus_Path);
            //_mainBus = RuntimeManager.GetBus(Main_Bus_Path);
            //_beforePlayerDeadSnapshot = RuntimeManager.CreateInstance("snapshot:/BeforePlayerDeadSnapshot");
        }

        public void SetGameSoundVolume(float volume, float duration)
        {
            SetVCAVolume(_subVCA, volume, duration);
        }

        public void SetGameSoundVolume(float volume)
        {
            SetVCAVolume(_subVCA, volume);
        }

        public void SetTimelineSoundVolume(float volume, float duration)
        {
            SetVCAVolume(_timelineVCA, volume, duration);
        }

        public void SetTimelineSoundVolume(float volume)
        {
            SetVCAVolume(_timelineVCA, volume);
        }

        public void SetBeforePlayerDead(bool isBeforePlayerDead)
        {
            if (isBeforePlayerDead)
            {
                _beforePlayerDeadSnapshot.start();
            }
            else
            {
                _beforePlayerDeadSnapshot.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }

        public void PauseMainSound()
        {
            _gameBus.setPaused(true);
        }

        public void PlayTextClickSound()
        {
            RuntimeManager.PlayOneShot("event:/UI/TextClick");
        }

        public void PlayButtonClickSound()
        {
            RuntimeManager.PlayOneShot("event:/UI/ButtonClick");
        }

        public void ResumeMainSound()
        {
            _gameBus.setPaused(false);
        }

        public void StopMusicSound()
        {
            if (MusicEventInstance.isValid())
            {
                MusicEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                MusicEventInstance.release();
            }
        }

        public void PlayMusicSound(EventReference musicEventReference)
        {
            StopMusicSound();

            MusicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
            MusicEventInstance.start();
        }

        public void SetMainVolume(float volume, float duration)
        {
            SetBusVolume(_mainBus, volume, duration);
        }

        public void SetMuteSound(bool muted)
        {
            Debug.Log(_mainBus.setMute(muted));
        }

        public void SetMainVolume(float volume)
        {
            SetBusVolume(_mainBus, volume);
        }

        public void SetUIVolume(float volume, float duration)
        {
            SetBusVolume(_uiBus, volume, duration);
        }

        public void SetUIVolume(float volume)
        {
            SetBusVolume(_uiBus, volume);
        }

        public void SetMusicVolume(float volume, float duration)
        {
            Debug.Log(_musicBus);
            SetBusVolume(_musicBus, volume, duration);
        }

        public void SetMusicVolume(float volume)
        {
            SetBusVolume(_musicBus, volume);
        }

        public void SetSFXVolume(float volume, float duration)
        {
            SetBusVolume(_sfxBus, volume, duration);
        }

        public void SetSFXVolume(float volume)
        {
            SetBusVolume(_sfxBus, volume);
        }

        private void SetBusVolume(Bus bus, float volume, float duration)
        {
            DOTween.To(() =>
            {
                bus.getVolume(out var curVolume);
                return curVolume;
            }, x => bus.setVolume(x), volume, duration).SetUpdate(true);
        }

        private void SetBusVolume(Bus bus, float volume)
        {
            bus.setVolume(volume);
        }

        private void SetVCAVolume(VCA vca, float volume, float duration)
        {
            DOTween.To(() =>
            {
                vca.getVolume(out var curVolume);
                return curVolume;
            }, x => vca.setVolume(x), volume, duration).SetUpdate(true);
        }

        private void SetVCAVolume(VCA vca, float volume)
        {
            vca.setVolume(volume);
        }
    }
}