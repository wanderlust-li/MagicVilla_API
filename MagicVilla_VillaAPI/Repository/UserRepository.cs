using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;

namespace MagicVilla_VillaAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    private string secretKey;

    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }


    public bool IsUniqueUser(string username)
    {
        var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == username);
        if (user == null)
        {
            return true;
        }

        return false;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto)
    {
        var user = _db.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower()
                                                      && u.Password == loginRequestDto.Password);

        if (user == null)
        {
            return new LoginResponseDTO()
            {
                Token = "",
                User = null 
            };
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDTO loginResponseDto = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
            User = user
        };

        return loginResponseDto;
    }

    public async Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO)
    {
        LocalUser user = new LocalUser()
        {
            UserName = registerationRequestDTO.UserName,
            Password = registerationRequestDTO.Password,
            Name = registerationRequestDTO.Name,
            Role = registerationRequestDTO.Role
        };

        _db.LocalUsers.Add(user);
        await _db.SaveChangesAsync();
        user.Password = "";
        return user;
    }
}