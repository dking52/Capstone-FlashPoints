using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashPoints.Models
{
    public class PrizeIndexViewModel
    {
        public User user { get; set; }
        public List<Prize> prizes { get; set; }
    }
}
