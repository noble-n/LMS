using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; } 
        public string ISBN { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime ExpectedCheckInDate { get; set; }
        public bool IsAvailable { get; set; }
        public int CheckoutDuration { get; set; }
        public double DefaultAmount { get; set; }
        public string CheckOutRef { get; set; }

    }
}
