﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookStore.Contract.Model
{
    public class LibraryItem
    {
        public int UserId { get; set; }
        public int BookId { get; set; }

        public User User { get; set; }

        public Book Book { get; set; }
    }
}
