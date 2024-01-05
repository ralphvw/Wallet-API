using System.ComponentModel.DataAnnotations;

namespace WalletWebApi.Models.Dto;

public class AddWalletDto
{
    [Required]
    [MinLength(1, ErrorMessage ="Code is a minimum of one character")]
    [MaxLength(255, ErrorMessage ="Code is a maximum of 255 characters")]
    public string Name { get; set; }
    [Required]
    [RegularExpression("^(momo|card)$", ErrorMessage = "Type must be 'momo' or 'card'")]
    public string Type { get; set; }
    [Required]
    [MinLength(10, ErrorMessage ="Code is a minimum of ten characters")]
    [MaxLength(16, ErrorMessage ="Code is a maximum of sixteen characters")]
    public string AccountNumber { get; set; }
    [Required]
    [RegularExpression("^(visa|mastercard|mtn|vodafone|airteltigo)$", ErrorMessage = "Invalid AccountScheme")]
    public string AccountScheme { get; set; }
}