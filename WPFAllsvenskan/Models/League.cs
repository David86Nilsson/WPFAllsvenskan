using System.Collections.Generic;

namespace WPFAllsvenskan.Models
{
    public class League : Competion
    {
        public string UrlSchedule { get; set; }
        public string UrlOdds { get; set; }
        public string UrlChances { get; set; }
        public string TxtSchedule { get; set; }
        public string TxtOdds { get; set; }
        public string TxtChances { get; set; }
        public List<Serie> Series { get; set; } = new();
        public League(string Name) : base()
        {
            base.Name = Name;
            TxtSchedule = Name + ".txt";
            TxtOdds = Name + "Odds.txt";
            TxtChances = Name + "Chances.txt";
        }
    }
}
