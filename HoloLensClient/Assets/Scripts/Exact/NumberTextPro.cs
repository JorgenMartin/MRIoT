using TMPro;
using UnityEngine;

namespace Exact
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class NumberTextPro : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro;

        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
            _textMeshPro.text = "";
        }

        public void SetNumber(int number)
        {
            if (number < 0)
            {
                _textMeshPro.text = "";
            }
            else
            {
                _textMeshPro.text = number.ToString();
            }
        }
    }
}
