using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System;
using FMOD;
using System.Threading.Tasks;

namespace Play
{
    #region Enum Define
    public enum SoundType 
    {
        Master = 0,
        SFX,
        BGM,
    }

    public enum PlayStyle
    {
        Normal,
        Highlight,
    }
    #endregion

    public class SoundManager : Singleton<SoundManager>
    {
        private FMOD.Studio.EventInstance bgmInstance;
        private FMOD.ChannelGroup masterChannelGroup;

        // Fmod Bus
        [Header("Fmod Bus")]
        private FMOD.Studio.Bus masterBus;
        private FMOD.Studio.Bus bgmBus;
        private FMOD.Studio.Bus sfxBus;

        [Header("MusicInfo")]
        private int startTimeSeconds = 60;
        public int currentPositionMs;

        private bool isPlaying = false;

        private void Start()
        {
            //FMOD �ʱ�ȭ
            Init();
        }

        #region Sound Init
        private void Init()
        {
            RuntimeManager.LoadBank("Master");
            RuntimeManager.LoadBank("BGM");
            RuntimeManager.LoadBank("SFX");

            // FMOD���� Bus ��������
            masterBus = RuntimeManager.GetBus("bus:/");
            bgmBus = RuntimeManager.GetBus("bus:/BGM");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");

            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        #region Set Sound Volume
        public void SetVolume(int soundType, float volume) 
        {
            switch (soundType)
            {
                case (int)SoundType.Master:
                    masterBus.setVolume(volume);
                    break;
                case (int)SoundType.SFX:
                    sfxBus.setVolume(volume);
                    break;
                case (int)SoundType.BGM:
                    bgmBus.setVolume(volume);
                    break;
            }

            #region ���� ���� Ȯ�� �׽�Ʈ �ڵ�
            //float masterVolume, sfxVolume, bgmVolume;
            //masterBus.getVolume(out masterVolume);
            //sfxBus.getVolume(out sfxVolume);
            //bgmBus.getVolume(out bgmVolume);
            //Debug.Log($"master ���� : {masterVolume}, SFX ���� : {sfxVolume}, BGM ���� : {bgmVolume}");
            #endregion
        }
        #endregion

        #region Play Music
        /// <summary>
        /// SoundManager �뷡 �ʱ⼳��
        /// </summary>
        /// <param name="songName"></param>
        public void SongInit(Song songName, PlayStyle style)
        {
            //�뷡 ���� ���ڿ� ��ȯ
            string songTitle = songName.ToString();

            //�뷡 �ҷ�����
            //FMOD ���� üũ �� �ʱ�ȭ
            if (bgmInstance.isValid())
            {
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.release();
            }

            //�뷡 �̺�Ʈ �ҷ�����
            string bgmEventPart = "event:/BGM/" + songTitle;
            bgmInstance = RuntimeManager.CreateInstance(bgmEventPart);

            //�뷡 ���̶���Ʈ ����
            if(style == PlayStyle.Highlight)
            {
                int startTimeMilliseconds = Mathf.RoundToInt(startTimeSeconds * 1000);
                bgmInstance.setTimelinePosition(startTimeMilliseconds);
            }
        }

        public void Play()
        {
            bgmInstance.start();
            isPlaying = true;
        }
        #endregion

        #region Pause Music
        /// <summary>
        /// �뷡 �Ͻ�����
        /// </summary>
        /// <param name="isPause">���߰� ������ true</param>
        public void SetPause(bool isPause)
        {
            if(isPlaying == isPause)
            {
                isPlaying = !isPause;
                bgmInstance.setPaused(isPause);
            }
            else
            {
                UnityEngine.Debug.LogError("�뷡 �÷��� ���� ��ħ");
            }
        }
        #endregion

        #region Check Music End
        public IEnumerator WaitForMusicEnd(Action onComplete)
        {
            if (!bgmInstance.isValid())
            {
                UnityEngine.Debug.LogError("BGM �ν��Ͻ��� ��ȿ���� �ʽ��ϴ�.");
                yield break;
            }

            FMOD.Studio.PLAYBACK_STATE state;

            do
            {
                bgmInstance.getPlaybackState(out state);
                yield return null; // �� �����Ӹ��� Ȯ��
            } while (state != FMOD.Studio.PLAYBACK_STATE.STOPPED);

            UnityEngine.Debug.Log("�뷡�� �������ϴ�!");
            onComplete?.Invoke(); // �ݹ� ����
        }
        #endregion

        public int GetTimelinePosition()
        {
            bgmInstance.getTimelinePosition(out currentPositionMs);

            return currentPositionMs;
        }
        
        public void End()
        {
            bgmInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            isPlaying = false;
        }
    }
}