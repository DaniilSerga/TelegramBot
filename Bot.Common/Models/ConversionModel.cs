using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Common.Models
{
    public class ConversionModel
    {
        public int UserId { get; set; }

        public string YtLink { get; set; } = string.Empty;

        public DateTime ConversionDate { get; set; } = DateTime.Now;
    }
}
