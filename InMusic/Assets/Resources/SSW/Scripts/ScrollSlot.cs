using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


namespace SongList {
    public class ScrollSlot : MonoBehaviour {
        [SerializeField] private Text titleText;
        [SerializeField] private Image highlightImage;

        private SongInfo _currentData;

        public void SetData(SongInfo data)
        {
            _currentData = data;
            if (data == null) {
                // 오류 발생 시 대비비
                titleText.text = "-----";
            }
            else {
                titleText.text = _currentData.Title; 
            }
        }

        public void SetHighlight(bool on) {
            if (highlightImage != null) {
                highlightImage.gameObject.SetActive(on);
            }
        }

    }
}