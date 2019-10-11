using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class ComboText : MonoBehaviour
    {
        private Text _comboText;

        void Awake()
        {
            _comboText = GetComponent<Text>();
            _comboText.enabled = false;
        }

        public void Draw(int combo) {
            if (combo <= 0) {
                _comboText.enabled = false;
                return;
            }

            if (combo < 5)
                return;

            _comboText.text = combo.ToString() + " Combo";
            _comboText.enabled = true;
        }
    }
}