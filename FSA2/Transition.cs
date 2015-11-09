using System;
using System.Text;

namespace FSA
{
    [Serializable]
    public class Transition
    {
        private State _to;
        private char _value;

        public Transition(char c, State to)
        {
            _to = to;
            _value = c;
        }

        public State To
        {
            get { return _to; }
            set { _to = value; }
        }

        public char Value
        {
            get { return _value; }
        }

        public void AppendDot(StringBuilder b)
        {
            b.Append(" -> ").Append(_to.Number).Append(" [label=\"");
            if (_value >= 0x21 && _value <= 0x7e && _value != '\\' && _value != '"')
            {
                b.Append(_value);
            }
            else
            {
                b.Append("\\u");
                string s = ((int)_value).ToString("x");
                if (_value < 0x10)
                {
                    b.Append("000").Append(s);
                }
                else if (_value < 0x100)
                {
                    b.Append("00").Append(s);
                }
                else if (_value < 0x1000)
                {
                    b.Append("0").Append(s);
                }
                else
                {
                    b.Append(s);
                }
            }
            b.Append("\"]\n");
        }
    }
}