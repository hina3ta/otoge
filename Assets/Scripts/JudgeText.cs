using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class JudgeText : MonoBehaviour
    {
        private static readonly Dictionary<Judge, Color> JudgeColor = new Dictionary<Judge, Color> {
            { Judge.Perfect, Color.yellow },
            { Judge.Great, Color.green },
            { Judge.Good, Color.blue },
            { Judge.Miss, Color.red },
        };

        private Text _judgeText;

        private float _elapsedTime;

        void Awake()
        {
            _judgeText = GetComponent<Text>();
            _judgeText.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_judgeText.IsActive())
                return;

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > 0.5f) {
                _judgeText.enabled = false;
            }
        }

        public void Draw(Judge judge) {
            _judgeText.text = judge.ToString();
            _judgeText.color = JudgeColor[judge];
            _judgeText.enabled = true;
            _elapsedTime = 0f;
        }
    }
}