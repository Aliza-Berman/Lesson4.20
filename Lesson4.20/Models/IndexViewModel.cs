using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lesson4._20.Data;

namespace Lesson4._20.Models
{
    public class IndexViewModel
    {
        public List<Ad> Ads { get; set; }
        public bool IsLoggedIn { get; set; }
        public int UserId { get; set; }
    }

}
