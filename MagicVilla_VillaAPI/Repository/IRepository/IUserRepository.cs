using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository.IRepository;

public interface IUserRepository
{
    bool IsUniqueUser(string username);

    Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto);

    Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO);
}