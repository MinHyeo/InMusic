using Fusion;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Play
{
    public class MultiTimelineController : NetworkBehaviour
    {
        [Networked] public int GameStartTickRaw { get; private set; }

        private const float Speed = 5f;

        public override void Spawned()
        {
            if (!Object.HasStateAuthority) return;
            GameStartTickRaw = -1;
        }
        
        /// <summary>
        /// 이제 이 함수는 각 스포너에게 "이 악보대로 각자 노트 생성을 시작하라"고 명령만 내립니다.
        /// </summary>
        public void StartGame(SongInfo songInfo, List<MultiNoteSpawner> spawners)
        {
            if (!Object.HasStateAuthority) return;

            float bpm = songInfo.BPM;
            float measureIntervalSec = (60f / bpm) * 4f;
            float startDelaySec = measureIntervalSec * 2f + 2f;

            int delayTicks = (int)(startDelaySec * Runner.TickRate);
            GameStartTickRaw = Runner.Tick.Raw + delayTicks;

            int globalNoteId = 0;

            // 모든 스포너에게 동일한 악보 정보와 시작 시간을 전달합니다.
            foreach (var spawner in spawners)
            {
                spawner.StartNoteGeneration(songInfo, GameStartTickRaw, Speed, ref globalNoteId);
            }
        }
    }
}

