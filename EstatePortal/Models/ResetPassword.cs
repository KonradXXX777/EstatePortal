using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EstatePortal.Models
{
    public class ResetPassword
    {
        [Required]
        public string Token { get; set; }

        [Required, DataType(DataType.Password), MinLength(8)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
