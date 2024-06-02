namespace Utils
{
    public static class NumberFormatter
    {
        public static string ToFormattedString(this double number)
        {
            if (number >= 1000000000)
            {
                return (number / 1000000000).ToString("F1") + "B";
            }
            if (number >= 1000000)
            {
                return (number / 1000000).ToString("F1") + "M";
            }
            if (number >= 1000)
            {
                return (number / 1000).ToString("F1") + "K";
            }
            return number.ToString("F1");
        }
    }
}