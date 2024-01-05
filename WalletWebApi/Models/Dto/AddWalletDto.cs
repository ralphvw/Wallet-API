using System.ComponentModel.DataAnnotations;

namespace WalletWebApi.Models.Dto;

public class AddWalletDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    [RegularExpression("^(momo|card)$", ErrorMessage = "Type must be 'momo' or 'card'")]
    public string Type { get; set; }
    [Required]
    public string AccountNumber { get; set; }
    [Required]
    [RegularExpression("^(visa|mastercard|mtn|vodafone|airteltigo)$", ErrorMessage = "Invalid AccountScheme")]
    public string AccountScheme { get; set; }
}