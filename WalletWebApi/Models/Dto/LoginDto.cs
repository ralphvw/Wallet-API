using System.ComponentModel.DataAnnotations;

namespace WalletWebApi.Models.Dto;

public class LoginDto
{
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}