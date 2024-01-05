using AutoMapper;
using WalletWebApi.Models.Domain;
using WalletWebApi.Models.Dto;

namespace WalletWebApi.Mappings;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Wallet, AddWalletDto>().ReverseMap();
        CreateMap<Wallet, WalletResponseDto>().ReverseMap();
    }
}