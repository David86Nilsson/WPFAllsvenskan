using System.Collections.Generic;

namespace WPFAllsvenskan.Models
{
    public class Competion
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; } = new();
        public List<Game> Games { get; set; } = new();
    }
}
