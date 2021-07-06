using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class CheckOut
    {
        public int CheckOutId { get; set; }
        public string BookName { get; set; }
        public int BookId { get; set; }
        public int StudentId { get; set; }
        public DateTime ExpectedCheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public double DefaultAmount { get; set; }
        public string CheckOutRef { get; set; }
        public double AmountDefaulted { get; set; }
        public bool IsCheckedIn { get; set; }
        public Book Book { get; set; }
        public Student Student { get; set; }
    }
}
