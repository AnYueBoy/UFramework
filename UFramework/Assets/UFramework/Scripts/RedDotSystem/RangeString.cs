using System;

namespace UFramework
{
    /// <summary>
    /// 范围字符串
    /// 表示在Source字符串中，从StartIndex到EndIndex范围的字符构成的字符串
    /// </summary>
    public class RangeString : IEquatable<RangeString>
    {
        /// <summary>
        /// 原字符串
        /// </summary>
        private string source;

        private int startIndex;
        private int endIndex;

        private int length;
        private bool isSourceNullOrEmpty;
        private int hashCode;

        public RangeString(string source, int startIndex, int endIndex)
        {
            this.source = source;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            length = endIndex - startIndex + 1;
            isSourceNullOrEmpty = string.IsNullOrEmpty(source);
            hashCode = 0;
        }

        public bool Equals(RangeString other)
        {
            bool isOtherNullOrEmpty = string.IsNullOrEmpty(other.source);

            if (isSourceNullOrEmpty && isOtherNullOrEmpty)
            {
                return true;
            }

            if (length != other.length)
            {
                return false;
            }

            for (int i = startIndex, j = other.startIndex; i <= endIndex; i++, j++)
            {
                if (source[i] != other.source[j])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (hashCode == 0 && !isSourceNullOrEmpty)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    hashCode = 31 * hashCode + source[i];
                }
            }

            return hashCode;
        }

        public override string ToString()
        {
            App.Make<IRedDotSystem>().CacheStringBuilder.Clear();
            for (int i = startIndex; i <= endIndex; i++)
            {
                App.Make<IRedDotSystem>().CacheStringBuilder.Append(source[i]);
            }

            return App.Make<IRedDotSystem>().CacheStringBuilder.ToString();
        }
    }
}