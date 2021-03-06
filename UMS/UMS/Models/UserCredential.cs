//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Web.Mvc;

namespace UMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class UserCredential
    {
        public int User_Id { get; set; }

        [Required(ErrorMessage = "*Field is required")]
        [RegularExpression(@"(\S)+", ErrorMessage = "*Make sure you dont have whitespace")]
        [StringLength(12, ErrorMessage = "*Field can't exceed more than 12 characters")]
        [MaxLength(12, ErrorMessage = "*Password must be 12 characters")]
        [AllowHtml]
        public string Password { get; set; }

        public virtual UserInfo UserInfo { get; set; }
    }
}
