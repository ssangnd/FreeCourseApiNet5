using FreeCourseApiNet5.Data;
using FreeCourseApiNet5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourseApiNet5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly AppSetting _appSettings;

        public UserController(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Validate(LoginModel model)
        {
            var user = _context.NguoiDungs
                .SingleOrDefault(p=>p.UserName==model.UserName && model.Password==p.Password);
            if (user == null) //ko dung nguoi dung
                              //return Ok(new
                              //{
                              //    Sucess = false,
                              //    Message="Invalid username/password",
                              //}) ;

                return Ok(new ApiResponse
                {
                    Success=false,
                    Message="Invalid username/password",
                });
            //cap token
            var token = await GenerateToken(user);


            return Ok(new ApiResponse
            {
                Success=true,
                Message="Authenticate sucess",
                Data=token
            });
        }

        //private string GenerateToken(NguoiDung nguoiDung)
        //{
        //    var jwtTokenHandler = new JwtSecurityTokenHandler();

        //    var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

        //    var tokenDescription = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(ClaimTypes.Name,nguoiDung.HoTen),
        //            new Claim(ClaimTypes.Email, nguoiDung.Email),
        //            new Claim("UserName",nguoiDung.UserName),
        //            new Claim("Id",nguoiDung.Id.ToString()),

        //            //roles
        //            new Claim("TokenId",Guid.NewGuid().ToString()),
        //        }),
        //        Expires = DateTime.UtcNow.AddMinutes(1),
        //        SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes),
        //        SecurityAlgorithms.HmacSha512Signature)
        //    };

        //    var token = jwtTokenHandler.CreateToken(tokenDescription);

        //    return jwtTokenHandler.WriteToken(token);
        //}

        private async Task<TokenModel> GenerateToken(NguoiDung nguoiDung)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.Name,nguoiDung.HoTen),
                        //new Claim(ClaimTypes.Email, nguoiDung.Email),
                        new Claim(JwtRegisteredClaimNames.Email,nguoiDung.Email),
                        new Claim(JwtRegisteredClaimNames.Sub,nguoiDung.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim("UserName",nguoiDung.UserName),
                        new Claim("Id",nguoiDung.Id.ToString()),

                        //roles
                    }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes),
                SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken= jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            //Luu database
            var refreshTokenEntity = new RefreshToken
            {
                Id=Guid.NewGuid(),
                JwtId=token.Id,
                UserId=nguoiDung.Id ,
                Token=refreshToken,
                IsUsed=false,
                IsRevoked=false,
                IssuedAt=DateTime.UtcNow,
                ExpriedAt=DateTime.UtcNow.AddHours(1)
            };

             await _context.AddAsync(refreshTokenEntity);
             await _context.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng=RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenValidateParam = new TokenValidationParameters
            {
                //tu cap token
                ValidateIssuer = false,
                ValidateAudience = false,

                //ky token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime=false,//ko co ktra token het han
            };

            try
            {
                //check 1: AccessToken valid format
                var tokenInverification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam,
                    out var validatedToken
                    );

                //check 2: check alg
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                        StringComparison.InvariantCultureIgnoreCase
                        );
                    if(!result)//false 
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Invalid token",
                        });
                    }

                }
                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInverification.Claims.FirstOrDefault
                    (x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnitTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Access token has not yet expired"
                    });
                }
                //check 4: check refreshtoken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault
                    (x => x.Token == model.RefreshToken);

                if (storedToken == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refesh token does not exist"
                    });
                }

                //check 5: check refreshToken is used/rewored
                if (storedToken.IsUsed)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token has been used"
                    });
                }
                //bi thu hoi hay chua
                if (storedToken.IsRevoked)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token has been revoked"
                    });
                }

                //check 6: Access Token id == jwt in refresh token

                var jti = tokenInverification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Token doesn't match"
                    });

                //update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();
                //create new token
                var user =await  _context.NguoiDungs.SingleOrDefaultAsync(x=>x.Id==storedToken.UserId);
                var token = await GenerateToken(user);


                return Ok(new ApiResponse
                {
                    Success = true,
                    Message="Renew Token success",
                    Data=token,
                });

            }
            catch(Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Something went wrong",
                });
            }
        }

        private DateTime ConvertUnitTimeToDateTime(long utcExpireDate)
        {
            //1970
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
