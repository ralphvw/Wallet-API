using WalletWebApi.Models.Domain;
using WalletWebApi.Models.Dto;

namespace WalletWebApi.Repositories;

public interface IWalletRepository
{
    Task<Wallet> CreateWalletAsync(Wallet wallet);
    Task<Wallet?> DeleteWalletAsync(Wallet wallet);
    Task<Wallet?> GetWalletAsync(int id);
    Task<List<Wallet>> GetAllWalletsAsync(string userId);
    Task<bool> CheckDuplicateWallet(string type, string accountNumber);
    Task<bool> CheckUserLimit(string userId, int limit);
    Task<bool> WalletExists(int id);
    public bool IsWalletOwner(Wallet wallet, string userId);
}