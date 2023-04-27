using System.Text.RegularExpressions;

namespace newTolkuchka.Services
{
    public partial class CompareForSiteMapService : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            Regex regex = MyRegex();

            // run the regex on both strings            
            var xRegexResult = regex.Match(x);
            var yRegexResult = regex.Match(y);
            if (xRegexResult.Success && yRegexResult.Success)
            {
                int ix = int.Parse(xRegexResult.Groups[0].Value);
                int iy = int.Parse(yRegexResult.Groups[0].Value);
                if (ix > iy)
                    return 1;
                else if (ix < iy)
                    return -1;
                else
                    return 0;
            }
            return x.CompareTo(y);
        }

        [GeneratedRegex(@"\d+")]
        private static partial Regex MyRegex();
    }
}
