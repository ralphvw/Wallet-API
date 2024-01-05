using Microsoft.AspNetCore.Identity;

namespace WalletWebApi.Models.Domain;

public class ApplicationUser: IdentityUser
{
    public ICollection<Wallet> Wallets { get; set; }
}