using Microsoft.EntityFrameworkCore;
using WalletWebApi.Data;
using WalletWebApi.Models.Domain;

namespace WalletWebApi.Repositories;

public class WalletRepository: IWalletRepository
{
    private readonly WalletDbContext _dbContext;

    public WalletRepository(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<Wallet> CreateWalletAsync(Wallet wallet)
    {
        await _dbContext.Wallets.AddAsync(wallet);
        await _dbContext.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet?> DeleteWalletAsync(Wallet wallet)
    {
        _dbContext.Wallets.Remove(wallet);
        await _dbContext.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet?> GetWalletAsync(int id)
    {
        return await _dbContext.Wallets.FindAsync(id);
    }

    public async Task<List<Wallet>> GetAllWalletsAsync(string userId)
    {
        List<Wallet> userWallets = await _dbContext.Wallets
            .Where(w => w.OwnerId == userId)
            .ToListAsync();

        return userWallets;
    }

    public async Task<bool> CheckDuplicateWallet(string type, string accountNumber)
    {
        bool exists = await _dbContext.Wallets.AnyAsync(w => w.Type == type && w.AccountNumber == accountNumber);

        return exists;
    }

    public async Task<bool> CheckUserLimit(string userId, int limit = 5)
    {
        int walletCount = await _dbContext.Wallets.CountAsync(w => w.OwnerId == userId);
        
        return walletCount > limit;
    }
    
    public async Task<bool> WalletExists(int id)
    {
        bool exists = await _dbContext.Wallets.AnyAsync(w => w.ID == id);

        return exists;
    }

    public bool IsWalletOwner(Wallet wallet, string userId)
    {
        return wallet.OwnerId == userId;
    }
}