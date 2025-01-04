using EstatePortal.Models;
using EstatePortal.ViewModels;
using System.Collections.Generic;
using System;

namespace EstatePortal.ViewModels
{
    public class ListingFilterViewModel
    {
        public string Search { get; set; }
        public string Location { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinArea { get; set; }
        public decimal? MaxArea { get; set; }

        public PropertyType? PropertyType { get; set; }
        public UserRole? UserRole { get; set; }
        public SellOrRent? SellOrRent { get; set; }

        public List<Announcement> Announcements { get; set; } = new List<Announcement>();
    }
}
