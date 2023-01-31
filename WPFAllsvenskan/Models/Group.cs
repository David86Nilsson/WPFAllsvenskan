using System.Collections.Generic;

namespace WPFAllsvenskan.Models
{
    internal class Group
    {
        public List<Team> Teams { get; set; } = new();
        public List<Game> Games { get; set; } = new();
    }
}
