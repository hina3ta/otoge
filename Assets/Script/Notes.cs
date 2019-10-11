using UnityEngine;

namespace Script
{
    public class Notes : MonoBehaviour
    {
        public struct Param
        {
            public Line line { get; private set; }
            public float time { get; private set; }

            public Param (Line line, float time) {
                this.line = line;
                this.time = time;
            }
        }

        public Param param { get; private set; }

        public void Initialize(Param param) {
            this.param = param;
        }
    }
}