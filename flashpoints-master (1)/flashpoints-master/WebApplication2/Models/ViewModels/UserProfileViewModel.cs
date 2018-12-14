using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashPoints.Models
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<Event> Events { get; set; }
        public List<Prize> Prizes { get; set; }
    }
}
