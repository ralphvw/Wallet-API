namespace WalletWebApi.Models.Dto;

public class AddWalletDto
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string AccountNumber { get; set; }
    public string AccountScheme { get; set; }
}