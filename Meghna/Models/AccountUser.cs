using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Models
{
    public class SignUpUserModel
    {
        [DisplayName("Name")]
        [StringLength(30)]
        public string Name { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Email")]        
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]        
        public string PhoneNumber { get; set; }

        [DisplayName("Address")]
        [StringLength(200)]
        public string Address { get; set; }

        [DisplayName("Password")]
        [StringLength(20)]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [StringLength(20)]
        [DataType(DataType.Password)]
        [Required]
        [Compare("Password",ErrorMessage ="Password not match")]
        public string ConfirmPassword { get; set; }

    }

    public class SignInUserModel
    {
        [DisplayName("Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }

    }

    public class ApplicationUser : IdentityUser
    {
        [DisplayName("Name")]
        [StringLength(50)]
        public string Name { get; set; }       

        [DisplayName("Birth Date")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Address")]
        [StringLength(200)]
        public string Address { get; set; }

    }

    public class ChangePasswordModel
    {


        [DisplayName("Crrent Password")]
        [DataType(DataType.Password)]
        [Required]
        public string CurrentPassword { get; set; }

        [DisplayName("New Password")]
        [DataType(DataType.Password)]
        [Required]
        public string NewPassword { get; set; }

        [DisplayName("Confirm Pasword")]
        [DataType(DataType.Password)]
        [Required]
        [Compare("NewPassword", ErrorMessage = "Password not match")]
        public string ConfirmPassword { get; set; }




    }

    public class EmailConfrimModel
    {
        public string Email { get; set; }
        //public bool IsConfirmed { get; set; }
        public bool EmailSent { get; set; }
        public bool EmailVerified { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required, EmailAddress, Display(Name = "Registered email address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
        public bool IsSuccess { get; set; }

    }



    // Role view model
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }

    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }

    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            //Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string City { get; set; }

        //public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }

    public class RegisterWithRole
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Please Select A Role")]

        public string RoleName { get; set; }
    }

    public class RoleUserViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }

}
