using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bot.Model.DatabaseModels
{
    [Table("Conversions")]
    public class Conversion
    {
        [Key]
        public int Id { get; set; }

        public long UserId { get; set; }

        public string YtLink { get; set; } = string.Empty;

        public DateTime ConversionDate { get; set; } = DateTime.Now;
    }
}
