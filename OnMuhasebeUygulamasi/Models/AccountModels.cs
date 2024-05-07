using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnMuhasebeUygulamasi.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Gerekli!")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Elektronik posta adresi bilgisi gereklidir!")]
        //[RegularExpression(@"^([a-zA-Z0-9_-.]+)@(([[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.)|(([a-zA-Z0-9-]+.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(]?)$", ErrorMessage = "Geçerli mail adresi girmeden geçiş yok!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Gerekli!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Gerekli!")]
        [Compare("Password", ErrorMessage = "Şifreler aynı değil!")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Şifreler aynı değil!")]
        public string ConfirmPassword { get; set; }
    }

    public class UserDetailsModel
    {
        public Guid UDid { get; set; }
        public int? Roleid { get; set; }
        public string RoleName { get; set; }

        public List<Role> RoleList { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }
    }
}