using WalletWebApi.Models.Domain;

namespace WalletWebApi.Models.Dto;

public class WalletResponseDto
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string AccountNumber { get; set; }
    public string AccountScheme { get; set; }
    public ApplicationUser Owner { get; set; }
}