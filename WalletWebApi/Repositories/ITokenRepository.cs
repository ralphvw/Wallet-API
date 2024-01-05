using Microsoft.AspNetCore.Identity;
using WalletWebApi.Models.Domain;

namespace WalletWebApi.Repositories;

public interface ITokenRepository
{
    string CreateJwtToken(ApplicationUser user);
}