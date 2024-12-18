using System.Collections.Generic;

namespace EstatePortal.Models
{
    public class AdminPanelViewModel
    {
        public List<User> Users { get; set; }
        public User EditUser { get; set; }
        public List<Announcement> Announcements { get; set; }
        public Announcement EditAnnouncement { get; set; }
    }
}