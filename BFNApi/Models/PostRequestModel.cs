using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BFNApi.Models
{
    public class PostRequestModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "First name must only contain alphabets.")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Last name must only contain alphabets.")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [MinLength(13, ErrorMessage = "Mobile number must be 10 characters.")]
        [MaxLength(13, ErrorMessage = "Mobile number must be 10 characters.")]
        [RegularExpression(@"^(\+639)\d{9}$", ErrorMessage = "Wrong mobile number format.")]
        public string mobileNumber { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [RegularExpression("^[0-9A-Za-z ]+$", ErrorMessage = "City is required.")]
        public string city { get; set; }

        [Required(ErrorMessage = "Respiratory condition is required.")]
        [RegularExpression("^[0-9A-Za-z ]+$", ErrorMessage = "Respiratory condition must only contain alphabets.")]
        public string respiratoryCondition { get; set; }

    }
}