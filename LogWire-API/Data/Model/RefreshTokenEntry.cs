using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LogWire.API.Data.Model
{
    public class RefreshTokenEntry
    {
        [Key]
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
