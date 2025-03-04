using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace Play
{
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

    public class SoundManager : SingleTon<SoundManager>
    {
        // ���� ��� X
        // FMOD Variable 
        [Header("music")]
        FMOD.System fmodSystem;
        FMOD.ChannelGroup musicChannelGroup;
        FMOD.Sound musicSound;
        FMOD.Channel musicChannel;

        private EventInstance bgmInstance;
        private FMOD.ChannelGroup masterChannelGroup;

        // Fmod Bus
        [Header("Fmod Bus")]
        private FMOD.Studio.Bus masterBus;
        private FMOD.Studio.Bus bgmBus;
        private FMOD.Studio.Bus sfxBus;

        [Header("MusicInfo")]
        public float frequency;
        private int startTimeSeconds = 60;
        public uint positionInSamples;

        private bool isPlaying = false;

        private void Start()
        {
            //FMOD �ʱ�ȭ
            Init();
        }

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

            //���� ���� Ȯ�� �׽�Ʈ �ڵ�
            //float masterVolume, sfxVolume, bgmVolume;
            //masterBus.getVolume(out masterVolume);
            //sfxBus.getVolume(out sfxVolume);
            //bgmBus.getVolume(out bgmVolume);
            //Debug.Log($"master ���� : {masterVolume}, SFX ���� : {sfxVolume}, BGM ���� : {bgmVolume}");
        }

        private void LoadSong(string songName)
        {
            //�뷡 �ҷ�����
            if (bgmInstance.isValid())
            {
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.release();
            }
            string bgmEventPart = "event:/BGM/" + songName;
            bgmInstance = RuntimeManager.CreateInstance(bgmEventPart);
        }

        public void PlayBGM(string songName, PlayStyle style)
        {
            // �뷡 �ҷ�����
            LoadSong(songName);

            // �뷡 ���̶���Ʈ ����
            if(style == PlayStyle.Highlight)
            {
                int startTimeMilliseconds = Mathf.RoundToInt(startTimeSeconds * 1000);
                bgmInstance.setTimelinePosition(startTimeMilliseconds);
            }

            //�뷡 ����
            bgmInstance.start();
            // ä�� �׷� ��������
            bgmInstance.getChannelGroup(out masterChannelGroup);
        }

        //public void PlayBGM(string bgmEventPart, PlayStyle style)
        //{
        //    bgmInstance.start();

        //    isPlaying = true;
        //}

        //public void PlayBGMHighLight(string songName)
        //{
        //    LoadSong(songName);

        //    //bgmInstance.setParameterByName("BGM_Loop", 1.0f);

        //    int startTimeMilliseconds = Mathf.RoundToInt(startTimeSeconds * 1000); // 44.1kHz ����
        //    bgmInstance.setTimelinePosition(startTimeMilliseconds);

        //    bgmInstance.start();
        //}

        public void Pause(bool isPause)
        {
            musicChannel.setPaused(isPause);
        }

        //public void SongInit(string songName)
        //{
        //    //FMOD �ʱ�ȭ
        //    //Init();

        //    //�뷡 �ҷ�����
        //    LoadSong(songName);

        //    //���� ���� ���
        //    frequency = GetCurrentFrequency();
        //}

        public float GetCurrentFrequency()
        {
            Debug.Log("dddddddd");
            if (masterChannelGroup.hasHandle())
            {
                Debug.Log("dddd");
                FMOD.DSP dsp;
                masterChannelGroup.getDSP(0, out dsp);  // DSP 0�� ��������

                // DSP�� ����Ǿ����� Ȯ��
                FMOD.DSP_PARAMETER_DESC paramDesc;
                dsp.getParameterInfo(0, out paramDesc);

                if(paramDesc.type == FMOD.DSP_PARAMETER_TYPE.FLOAT)
                {
                    dsp.getParameterFloat(0, out frequency);  // DSP���� ���ļ� ��������
                    Debug.Log($"SoundManager���� ���� ���� : {frequency}");
                }
                else
                {
                    Debug.LogError("DSP Parameter 0 type is not FLOAT");
                }     
            }

            return frequency;
        }

        private void Update()
        {
            if (!this.isPlaying)
                return;

            //���� ���ø� ���ļ� ���
            musicChannel.getPosition(out positionInSamples, FMOD.TIMEUNIT.PCM);

            //�뷡�� �������� üũ
            bool isPlaying;
            musicChannel.isPlaying(out isPlaying);

            if (!isPlaying)
                OnMusicEnd();

            fmodSystem.update();
        }

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