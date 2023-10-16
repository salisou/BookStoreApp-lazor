namespace bookstoreApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //L'attributo [Authorize] in ASP.NET Core viene utilizzato per proteggere
    //le risorse o i metodi delle API da accessi non autorizzati.
    //Quando applicato a una classe di controller o a un metodo di azione,
    //richiede che l'utente sia autenticato e autorizzato per accedere alla risorsa
    //[Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<ApiUser> userManager;
        private readonly IConfiguration configuration;

        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            logger.LogInformation($"Registration Attempt for {userDto.Email} ");
            
            try
            {
                var user = mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                var result = await userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }

                await userManager.AddToRoleAsync(user, "User");

                return Accepted();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Went Worng in the {nameof(Register)}");
                return Problem($"Something Went Worng in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginUserDto userDto)
        {
            logger.LogInformation($"Login Attempt for {userDto.Email} ");

            try
            {
                var _user = await userManager.FindByEmailAsync(userDto.Email);
                var _posswordValid = await userManager.CheckPasswordAsync(_user, userDto.Password);

                if (_user == null || _posswordValid == false)
                    return Unauthorized(userDto);

                string tokenSring = await GenerateToken(_user);

                var response = new AuthResponse
                {
                    Email = userDto.Email,
                    Token = tokenSring,
                    UserId = _user.Id
                };

                return Ok(response);
            } 
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Went Worng in the {nameof(Register)}");
                return Problem($"Something Went Worng in the {nameof(Register)}", statusCode: 500);
            }
        }

        private async Task<string> GenerateToken(ApiUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await userManager.GetRolesAsync(user);
            var rolesClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim(CustomClaimTypes.Uid, user.Id),
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var token = new JwtSecurityToken(
                    issuer: configuration["JwtSettings:Issuer"],
                    audience: configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(Convert.ToInt32(configuration["JwtSettings:Duration"])),
                    signingCredentials: credentials

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
