using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class LibraryVM
    {
        public string CheckOutRefNo { get; set; }
        public string CheckOutNotNull { get; set; }
        public CheckOut CheckOut { get; set; }
    }
}
