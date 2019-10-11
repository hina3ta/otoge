using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script
{
    public class PlaySceneManager : MonoBehaviour
    {
        private static readonly List<Notes.Param> MusicMaster = new List<Notes.Param>() {
            new Notes.Param(Line.Line1, 1f),
            new Notes.Param(Line.Line2, 2f),
            new Notes.Param(Line.Line3, 3f),
            new Notes.Param(Line.Line4, 4f),
            new Notes.Param(Line.Line1, 5f),
            new Notes.Param(Line.Line2, 6f),
            new Notes.Param(Line.Line3, 7f),
            new Notes.Param(Line.Line4, 8f),
        };

        private static readonly Dictionary<Line, float> LinePos = new Dictionary<Line, float>() {
            { Line.Line1, -6f },
            { Line.Line2, -2f },
            { Line.Line3, 2f },
            { Line.Line4, 6f },
        };

        private static readonly Dictionary<Judge, float> JudgeThreshold = new Dictionary<Judge, float>() {
            { Judge.Perfect, 0.1f },
            { Judge.Great, 0.3f },
            { Judge.Good, 0.5f },
            { Judge.Miss, 1f },
        };

        private static readonly Dictionary<Judge, int> JudgeScore = new Dictionary<Judge, int>() {
            { Judge.Perfect, 1000 },
            { Judge.Great, 500 },
            { Judge.Good, 100 },
            { Judge.Miss, 0 },
        };

        private static readonly float BASE_SPEED = 10f;

        [FormerlySerializedAs("NotesPrefab")] [SerializeField] private GameObject notesPrefab;
        [SerializeField] private GameObject judgeLineObj;
        [SerializeField] private AudioSource playMusic;
        [SerializeField] private AudioSource soundEffect;
        [SerializeField] private Text scoreText;
        [SerializeField] private JudgeText judgeText;
        [SerializeField] private ComboText comboText;

        private Dictionary<Line, List<Notes>> _notesDic;
        private Dictionary<Line, int> _currentNotesIndexDic;

        private int _score;
        private int _combo;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Return)) {
            //    SceneManager.LoadScene("ResultScene");
            //}

            // ノート移動処理
            foreach (var notes in _notesDic.Values) {
                foreach (var note in notes) {
                    var transform1 = note.transform;
                    var pos = transform1.position;
                    pos.y = judgeLineObj.transform.position.y + (note.param.time - playMusic.time) * BASE_SPEED;
                    transform1.position = pos;
                }
            }

            // 見逃しミス判定
            for (int i = 0; i < _currentNotesIndexDic.Count; ++i) {
                var line = (Line)i;
                var index = _currentNotesIndexDic[line];
                if (index < 0)
                    continue;

                var note = _notesDic[line][index];
                var diff = note.param.time - playMusic.time;
                if (diff < -JudgeThreshold[Judge.Miss]) {
                    // ミス
                    _combo = 0;

                    judgeText.Draw(Judge.Miss);
                    comboText.Draw(_combo);

                    note.gameObject.SetActive(false);

                    if (index + 1 < _notesDic[line].Count) {
                        ++_currentNotesIndexDic[line];
                    } else {
                        _currentNotesIndexDic[line] = -1;
                    }
                }
            }

            // キー入力
            if (Input.GetKeyDown(KeyCode.A)) {
                JudgeNote(Line.Line1);
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                JudgeNote(Line.Line2);
            }
            if (Input.GetKeyDown(KeyCode.D)) {
                JudgeNote(Line.Line3);
            }
            if (Input.GetKeyDown(KeyCode.F)) {
                JudgeNote(Line.Line4);
            }
        }

        void Initialize() {
            _notesDic = new Dictionary<Line, List<Notes>>();
            _currentNotesIndexDic = new Dictionary<Line, int>();
            foreach (Line line in System.Enum.GetValues(typeof(Line))) {
                _notesDic.Add(line, new List<Notes>());
                _currentNotesIndexDic.Add(line, -1);
            }
            _score = 0;
            _combo = 0;

            // ノート生成
            foreach (var master in MusicMaster) {
                var obj = Instantiate(notesPrefab, transform);
                var note = obj.GetComponent<Notes>();
                note.Initialize(master);

                var pos = obj.transform.position;
                pos.x = LinePos[note.param.line];
                pos.y = judgeLineObj.transform.position.y + (BASE_SPEED * note.param.time);
                obj.transform.position = pos;

                _notesDic[note.param.line].Add(note);
                if (_currentNotesIndexDic[note.param.line] < 0) {
                    _currentNotesIndexDic[note.param.line] = 0;
                }
            }

            playMusic.Play();
        }

        void JudgeNote(Line line) {
            soundEffect.Play();

            int index = _currentNotesIndexDic[line];
            if (index < 0)
                return;

            var note = _notesDic[line][index];
            var diff = Mathf.Abs(playMusic.time - note.param.time);
            if (diff > JudgeThreshold[Judge.Miss])
                return;

            var judge = Judge.Miss;
            if (diff < JudgeThreshold[Judge.Perfect]) {
                judge = Judge.Perfect;
            } else if (diff < JudgeThreshold[Judge.Great]) {
                judge = Judge.Great;
            } else if (diff < JudgeThreshold[Judge.Good]) {
                judge = Judge.Good;
            }

            if (judge != Judge.Miss) {
                _score += JudgeScore[judge];
                scoreText.text = "Score: " + _score.ToString("D7");
                ++_combo;
            } else {
                _combo = 0;
            }

            judgeText.Draw(judge);
            comboText.Draw(_combo);

            note.gameObject.SetActive(false);

            if (index + 1 < _notesDic[line].Count) {
                ++_currentNotesIndexDic[line];
            } else {
                _currentNotesIndexDic[line] = -1;
            }
        }
    }
}
