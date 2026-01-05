using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCOLL.Data;

namespace MyCOLL.Gestao.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("Account/Login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            // Tenta fazer login com a password
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return LocalRedirect("/"); // Se correu bem, vai para a Home
            }

            // Se falhou, volta para a página de login com erro (usamos um parâmetro na URL)
            return LocalRedirect("/login?erro=true");
        }

        [HttpPost("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/");
        }
    }
}