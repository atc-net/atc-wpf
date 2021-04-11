using System;
using System.Windows;
using System.Xml;

namespace Atc.Wpf.Controls.W3cSvg
{
    internal class StringSplitter
    {
        private string val;
        private int curPos;

        public StringSplitter(string value)
        {
            this.val = value;
            this.curPos = 0;
        }

        public void SetString(string value, int startPos)
        {
            this.val = value;
            this.curPos = startPos;
        }

        public bool More => this.curPos >= 0 && this.curPos < this.val.Length;

        public double ReadNextValue()
        {
            int startPos = this.curPos;
            if (startPos < 0)
            {
                startPos = 0;
            }

            if (startPos >= this.val.Length)
            {
                return float.NaN;
            }

            string numbers = "0123456789-.eE";
            while (startPos < this.val.Length && !numbers.Contains(this.val[startPos], StringComparison.Ordinal))
            {
                startPos++;
            }

            int endPos = startPos;
            while (endPos < this.val.Length && numbers.Contains(this.val[endPos], StringComparison.Ordinal))
            {
                // '-' if number is followed by '-' then it indicates the end of the value
                if (endPos != startPos &&
                    this.val[endPos] == '-' &&
                    this.val[endPos - 1] != 'e' &&
                    this.val[endPos - 1] != 'E')
                {
                    break;
                }

                endPos++;
            }

            int len = endPos - startPos;
            if (len <= 0)
            {
                return float.NaN;
            }

            this.curPos = endPos;
            string s = this.val.Substring(startPos, len);

            startPos = endPos;
            while (startPos < this.val.Length && !numbers.Contains(this.val[startPos], StringComparison.Ordinal))
            {
                startPos++;
            }

            if (startPos >= this.val.Length)
            {
                endPos = startPos;
            }

            this.curPos = endPos;
            return XmlConvert.ToDouble(s);
        }

        public Point ReadNextPoint()
        {
            double x = this.ReadNextValue();
            double y = this.ReadNextValue();
            return new Point(x, y);
        }
    }
}