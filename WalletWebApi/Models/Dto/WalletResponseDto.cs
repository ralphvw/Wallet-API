using WalletWebApi.Models.Domain;

namespace WalletWebApi.Models.Dto;

public class WalletResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string AccountNumber { get; set; }
    public string AccountScheme { get; set; }
}