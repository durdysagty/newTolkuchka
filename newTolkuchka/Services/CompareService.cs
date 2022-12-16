using newTolkuchka.Models.DTO;
using System.Text.RegularExpressions;

namespace newTolkuchka.Services
{
    public partial class CompareService : IComparer<FilterValue>
    {
        public int Compare(FilterValue x, FilterValue y)
        {
            Regex regex = MyRegex();

            // run the regex on both strings
            var xRegexResult = regex.Match(x.Name);
            var yRegexResult = regex.Match(y.Name);

            // check if they are both numbers
            if (xRegexResult.Success && yRegexResult.Success)
            {
                string[] valuex = x.Name.Split(xRegexResult.Groups[0].Value);
                string[] valuey = y.Name.Split(yRegexResult.Groups[0].Value);
                if (valuex.Length > 1 && valuey.Length > 1)
                {
                    string[] compareValues = new string[] { "mb", "мб", "Mb", "Мб", "MB", "МБ", "gb", "гб", "Gb", "Гб", "GB", "ГБ", "tb", "тб", "Tb", "Тб", "TB", "ТБ", "pb", "пб", "Pb", "Пб", "PB", "ПБ" };
                    int indexx = Array.IndexOf(compareValues, valuex[1]);
                    int indexy = Array.IndexOf(compareValues, valuey[1]);
                    if (indexx > indexy)
                        return 1;
                    else if (indexx < indexy)
                        return -1;
                    else
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
                }                

                //return int.Parse(xRegexResult.Groups[0].Value).CompareTo(int.Parse(yRegexResult.Groups[0].Value));
            }

            // otherwise return as string comparison
            return x.Name.CompareTo(y.Name);
        }

        [GeneratedRegex(@"^\d+")]
        private static partial Regex MyRegex();
    }
}
