using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Core.Entities.User
{
    public class Bill
    {
        public int ID { get; set; }

        public int OwnerId { get; set; }
        public int UserId { get; set; }
        public string PlaceName { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public int PlaceId { get; set; }
    }
}
