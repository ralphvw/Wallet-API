using System.ComponentModel.DataAnnotations;

namespace WalletWebApi.Models.Dto;

public class RegisterDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string PhoneNumber { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}