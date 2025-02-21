using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SongList {
    public class IndexSaveTest : Singleton<IndexSaveTest> {
        private static int _lastSelectedIndex = -1;
        protected override void Awake() {
            base.Awake();
        }
        public void SelectSong(int index) {
            _lastSelectedIndex = index;
        }

        public int GetLastSelectedIndex() {
            Debug.Log("GetLastSelectedIndex: " + _lastSelectedIndex);
            return _lastSelectedIndex;
        }
    }
}