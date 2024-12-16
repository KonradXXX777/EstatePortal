using System.Collections.Generic;

namespace EstatePortal.Models
{
    public class AdminPanelViewModel
    {
        public List<User> Users { get; set; }
        public User EditUser { get; set; }
    }
}
