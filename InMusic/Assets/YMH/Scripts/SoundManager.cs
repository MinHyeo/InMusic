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

    public class SoundManager : SingleTon<SoundManager>
    {
        #region ���X
        // FMOD Variable 
        [Header("music")]
        FMOD.System fmodSystem;
        FMOD.ChannelGroup musicChannelGroup;
        FMOD.Sound musicSound;
        FMOD.Channel musicChannel;
        #endregion

        private FMOD.Studio.EventInstance bgmInstance;
        private FMOD.ChannelGroup masterChannelGroup;

        // Fmod Bus
        [Header("Fmod Bus")]
        private FMOD.Studio.Bus masterBus;
        private FMOD.Studio.Bus bgmBus;
        private FMOD.Studio.Bus sfxBus;

        [Header("MusicInfo")]
        public int frequency;
        private int startTimeSeconds = 60;
        public uint positionInSamples;

        private bool isPlaying = false;

        private void Start()
        {
            //FMOD �ʱ�ȭ
            Init();
        }

        #region Sound Init
        private void Init()
        {
            //fmodSystem = RuntimeManager.CoreSystem;
            //fmodSystem.createChannelGroup("Music", out musicChannelGroup);

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
        public async Task SongInit(Song songName, PlayStyle style)
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

            //ä�� �׷� ��������
            //StartCoroutine(GetChannelGroup());
            await RunCoroutineAsTask(GetChannelGroup());
        }

        private Task RunCoroutineAsTask(IEnumerator coroutine)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            StartCoroutine(CoroutineWrapper(coroutine, tcs));
            return tcs.Task;
        }

        private IEnumerator CoroutineWrapper(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
        {
            yield return coroutine;
            tcs.SetResult(true);
        }

        private IEnumerator GetChannelGroup()
        {
            //�뷡 �����ϰ� �ٷ� �Ͻ�����
            isPlaying = true;
            bgmInstance.start();
            Pause(!isPlaying);

            //������ ���
            yield return null;

            //ä�α׷� ��������
            FMOD.Studio.PLAYBACK_STATE state;
            do
            {
                yield return null;
                bgmInstance.getPlaybackState(out state);
            } while (state != FMOD.Studio.PLAYBACK_STATE.PLAYING);
            bgmInstance.getChannelGroup(out masterChannelGroup);

            //���� ���ϱ�
            GetCurrentFrequency();
        }

        public void Play()
        {
            bgmInstance.start();
            isPlaying = true;
        }
        #endregion

        #region Pause Music
        public void Pause(bool isPause)
        {
            if(isPlaying != isPause)
            {
                isPlaying = isPause;
                bgmInstance.setPaused(isPause);
            }
            else
            {
                UnityEngine.Debug.LogError("�뷡 �÷��� ���� ��ħ");
            }
        }
        #endregion

        #region Get Frequency
        public void GetCurrentFrequency()
        {
            if (!masterChannelGroup.hasHandle())
            {
                UnityEngine.Debug.LogError("Master Channel Group has no handle");
                return;
            }

            FMOD.System system;
            FMOD.RESULT result = masterChannelGroup.getSystemObject(out system);
            if (result != FMOD.RESULT.OK)
            {
                UnityEngine.Debug.LogError($"FMOD �ý��� �������� ����: {result}");
                return;
            }

            // ���� ���ø� ���ļ� (��: 44100 Hz)
            FMOD.SPEAKERMODE speakerMode;
            int numRawSpeakers;
            result = system.getSoftwareFormat(out frequency, out speakerMode, out numRawSpeakers);
            UnityEngine.Debug.Log($"frequency : {frequency}");
            if (result != FMOD.RESULT.OK)
            {
                UnityEngine.Debug.LogError($"FMOD ���ø� ���ļ� �������� ����: {result}");
                return;
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
        //private void Update()
        //{
        //    if (!this.isPlaying)
        //        return;

        //    //���� ���ø� ���ļ� ���
        //    musicChannel.getPosition(out positionInSamples, FMOD.TIMEUNIT.PCM);

        //    //�뷡�� �������� üũ
        //    bool isPlaying;
        //    musicChannel.isPlaying(out isPlaying);

        //    if (!isPlaying)
        //        OnMusicEnd();

        //    fmodSystem.update();
        //}
        #endregion

        private void OnMusicEnd()
        {
            UnityEngine.Debug.Log("�뷡 ��");
            PlayManager.Instance.End();
        }

        public void End()
        {
            musicChannel.stop();
            musicChannelGroup.release();

            musicSound.release();

            fmodSystem.close();
            fmodSystem.release();

            isPlaying = false;
        }
    }
}