using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMS.Models
{
    public class UserInfoViewModel
    {
        public int User_Id { get; set; }
        [Display(Name = "Username")]
        [StringLength(20, ErrorMessage = "Cant exceed more than 20 characters")]
        [MaxLength(20)]
        [Required(ErrorMessage = "Field is required")]
        [RegularExpression(@"[A-Za-z0-9]+", ErrorMessage = "Invalid username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        [StringLength(20, ErrorMessage = "Cant exceed more than 20 characters")]
        [MaxLength(20)]
        [Required(ErrorMessage = "Field is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(30, ErrorMessage = "Cant exceed more than 30 characters")]
        [MaxLength(30)]
        [Required(ErrorMessage = "Field is required")]
        public string LastName { get; set; }

        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Emial")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        [StringLength(50, ErrorMessage = "Cant exceed more than 50 characters")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Field is required")]
        public string Email { get; set; }

        [StringLength(80, ErrorMessage = "Cant exceed more than 80 characters")]
        [MaxLength(80)]
        [Required(ErrorMessage = "Field is required")]
        public string Address { get; set; }

        [StringLength(50, ErrorMessage = "Cant exceed more than 50 characters")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Field is required")]
        public string City { get; set; }

        [StringLength(50, ErrorMessage = "Cant exceed more than 50 characters")]
        [MaxLength(50)]
        public string Country { get; set; }

        [Display(Name = "Zip Code")]
        [StringLength(9, ErrorMessage = "Cant exceed more than 9 characters")]
        [MaxLength(9)]
        [RegularExpression("(^\\d{5}(-\\d{4})?$)|(^[ABCEGHJKLMNPRSTVXY]{1}\\d{1}[A-Z]{1} *\\d{1}[A-Z]{1}\\d{1}$)", ErrorMessage = "Invalid Zipcode")]
        [DataType(DataType.PostalCode, ErrorMessage = "Invalid Zipcode")]
        public string Zipcode { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(14, ErrorMessage = "Cant exceed more than 13 characters")]
        [MaxLength(14)]
        [RegularExpression(@"^((1-)?\d{3}-)?\d{3}-\d{4}$", ErrorMessage = "Invalid Phone Number")]
        //[DataType(DataType.PhoneNumber, ErrorMessage = "Invalid Phone Number")]
        [Required(ErrorMessage = "Field is required")]
        public string PhoneNumber { get; set; }

        public virtual ProfilePicture ProfilePicture { get; set; }
        public virtual Role Role { get; set; }
        public virtual Status Status { get; set; }
        public virtual UserCredential UserCredential { get; set; }

    }
    public class UserCredentialViewModel
    {
        [Required(ErrorMessage = "Field is required")]
        //[RegularExpression("^[^s,]+$(?=.*[a-z])(?=.*[A-Z])(?=.*d)(?=.*[^da-zA-Z]).{8,15}$", ErrorMessage = "Make sure you dont have whitespace or commas")]
        [DataType(DataType.Password, ErrorMessage = "Invalid characters")]
        [StringLength(12, ErrorMessage = "Field can't exceed more than 12 characters")]
        [MaxLength(12, ErrorMessage = "Password must be 12 characters")]
        public string Password { get; set; }

        public virtual UserInfo UserInfo { get; set; }
        
    }
}