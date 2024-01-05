using WalletWebApi.Models.Domain;
using WalletWebApi.Models.Dto;

namespace WalletWebApi.Repositories;

public interface IWalletRepository
{
    Task<Wallet> CreateWalletAsync(Wallet wallet);
    Task<Wallet> DeleteWalletAsync(Wallet wallet);
    Task<Wallet> GetWalletAsync(int id);
    Task<Wallet> GetAllWalletsAsync(string userId);
}