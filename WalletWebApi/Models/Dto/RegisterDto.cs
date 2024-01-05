using System.ComponentModel.DataAnnotations;

namespace WalletWebApi.Models.Dto;

public class RegisterDto
{
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}