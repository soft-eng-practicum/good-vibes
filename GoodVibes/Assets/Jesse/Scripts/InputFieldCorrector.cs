using UnityEngine;
using UnityEngine.UI;

namespace A
{
    /// <summary>
    /// Input field corrector. intended to work with the change of input and input end functions.
    /// </summary>
    public class InputFieldCorrector : MonoBehaviour
    {
        private InputField m_TargetInputField;

        //public bool m_PermitLettersAndDigits;
        public bool m_PermitDigits;
        public bool m_PermitLetters;
        public bool m_PermitSpaces;
        public bool m_PermitPeriod;
        public bool m_PermitPunctuation;


        // Use this for initialization
        void Start()
        {
            m_TargetInputField = GetComponent<InputField>();
        }
        //      
        //        // Update is called once per frame
        //        void Update () {
        //          
        //        }

        /// <summary>
        /// Corrects the input of the inputfield. call on edit and on edit end.
        /// </summary>
        public void OnValueChangedFunction()
        {
            if (m_TargetInputField.text.Length > 0)
            {
                char c = m_TargetInputField.text[m_TargetInputField.text.Length - 1]; // get last character
                /*if (!m_PermitSpecialChars && (c != '!' && c != '@' && c != '#' && c != '%' && c != '&' && c != '*' && c != '(' && c != ')' && c != '+' && c != '=' && c != '-' && c != '_'))
                {
                    CullLastChar();
                    return;
                }*/
                if (char.IsDigit(c) && !m_PermitDigits)
                {
                    CullLastChar();
                    return;
                }
                if (char.IsLetter(c) && !m_PermitLetters)
                {
                    CullLastChar();
                    return;
                }
                if (char.IsWhiteSpace(c) && !m_PermitSpaces)
                {
                    CullLastChar();
                    return;
                }
                /*if (c != '.' && !m_PermitPeriod)
                {
                    CullLastChar();
                    return;
                }*/
                if (char.IsPunctuation(c) && !m_PermitPunctuation && (c != '.' && c != '!' && c != '?' && c != '@' && c != '#' && c != '%' && c != '&' && c != '*' && c != '(' && c != ')' && c != '+' && c != '=' && c != '-' && c != '_' && c != '<' && c != '>'))
                {
                    CullLastChar();
                    return;
                }

            }
        }
        private void CullLastChar()
        {
            m_TargetInputField.text = m_TargetInputField.text.Remove(m_TargetInputField.text.Length - 1);
        }
    }
}