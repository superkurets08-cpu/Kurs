using System.Security.Claims;
using KURS_ASP.NET.Data;
using KURS_ASP.NET.Models;
using KURS_ASP.NET.Services.Interfaces;
using KURS_ASP.NET.Utilities;
using KURS_ASP.NET.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public AccountController(
            ApplicationDbContext context,
            IPasswordHasherService passwordHasher,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _environment = environment;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["GoogleLoginEnabled"] = IsGoogleAuthConfigured();
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["GoogleLoginEnabled"] = IsGoogleAuthConfigured();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.UserName == model.UserName && u.IsActive);

            if (user == null || !_passwordHasher.VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return View(model);
            }

            await SignInAsync(user, model.RememberMe);
            TempData["SuccessMessage"] = $"Здравствуйте, {user.UserName}.";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["GoogleLoginEnabled"] = IsGoogleAuthConfigured();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["GoogleLoginEnabled"] = IsGoogleAuthConfigured();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userName = model.UserName.Trim();
            var email = model.Email.Trim();

            if (await _context.AppUsers.AnyAsync(u => u.UserName == userName))
            {
                ModelState.AddModelError(nameof(model.UserName), "Пользователь с таким логином уже существует.");
                return View(model);
            }

            if (await _context.AppUsers.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError(nameof(model.Email), "Пользователь с такой почтой уже существует.");
                return View(model);
            }

            var user = new AppUser
            {
                UserName = userName,
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(model.Password),
                Role = SecurityRoles.User,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            await SignInAsync(user, rememberMe: false);
            TempData["SuccessMessage"] = "Регистрация выполнена. Теперь вам доступен расширенный функционал.";

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            return View(new ProfileViewModel
            {
                AppUserId = user.AppUserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                CreatedAtUtc = user.CreatedAtUtc,
                AvatarUrl = user.AvatarUrl,
                CommentsCount = await _context.TournamentComments.CountAsync(c => c.AppUserId == user.AppUserId),
                AvailablePresetAvatars = PresetAvatarCatalog.All
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                user.AvatarUrl = await SaveAvatarAsync(model.AvatarFile, "users", $"user-{user.AppUserId}");
            }
            else if (PresetAvatarCatalog.Contains(model.SelectedPresetAvatar))
            {
                user.AvatarUrl = model.SelectedPresetAvatar;
            }
            else
            {
                TempData["ErrorMessage"] = "Выберите файл или одну из готовых аватарок.";
                return RedirectToAction(nameof(Profile));
            }

            await _context.SaveChangesAsync();
            await SignInAsync(user, rememberMe: true);
            TempData["SuccessMessage"] = "Аватарка профиля обновлена.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Вы вышли из аккаунта.";
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            if (!string.Equals(provider, GoogleDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                TempData["ErrorMessage"] = "Неизвестный внешний провайдер авторизации.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            if (!IsGoogleAuthConfigured())
            {
                TempData["ErrorMessage"] = "Вход через Google пока не настроен. Добавьте ClientId и ClientSecret в конфиг.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Action("Index", "Home");

            if (!string.IsNullOrWhiteSpace(remoteError))
            {
                TempData["ErrorMessage"] = $"Ошибка внешней авторизации: {remoteError}";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            var externalResult = await HttpContext.AuthenticateAsync("External");
            if (!externalResult.Succeeded || externalResult.Principal == null)
            {
                TempData["ErrorMessage"] = "Не удалось получить данные пользователя от Google.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            var provider = externalResult.Properties?.Items.TryGetValue(".AuthScheme", out var authScheme) == true
                ? authScheme
                : GoogleDefaults.AuthenticationScheme;

            var subject = externalResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? externalResult.Principal.FindFirstValue("sub");
            var email = externalResult.Principal.FindFirstValue(ClaimTypes.Email);
            var displayName = externalResult.Principal.FindFirstValue(ClaimTypes.Name);
            var picture = externalResult.Principal.FindFirstValue("picture");

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(email))
            {
                await HttpContext.SignOutAsync("External");
                TempData["ErrorMessage"] = "Google не вернул обязательные данные профиля.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            var normalizedEmail = email.Trim();
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u =>
                    u.IsActive &&
                    u.ExternalProvider == provider &&
                    u.ExternalSubject == subject);

            if (user == null)
            {
                user = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.IsActive && u.Email == normalizedEmail);

                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = await GenerateUniqueUserNameAsync(displayName, normalizedEmail),
                        Email = normalizedEmail,
                        PasswordHash = string.Empty,
                        Role = SecurityRoles.User,
                        IsActive = true,
                        CreatedAtUtc = DateTime.UtcNow
                    };

                    _context.AppUsers.Add(user);
                }

                user.ExternalProvider = provider;
                user.ExternalSubject = subject;

                if (string.IsNullOrWhiteSpace(user.AvatarUrl) && !string.IsNullOrWhiteSpace(picture))
                {
                    user.AvatarUrl = picture;
                }

                await _context.SaveChangesAsync();
            }
            else if (string.IsNullOrWhiteSpace(user.AvatarUrl) && !string.IsNullOrWhiteSpace(picture))
            {
                user.AvatarUrl = picture;
                await _context.SaveChangesAsync();
            }

            await SignInAsync(user, rememberMe: true);
            await HttpContext.SignOutAsync("External");
            TempData["SuccessMessage"] = $"Здравствуйте, {user.UserName}.";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task<AppUser?> GetCurrentUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out var parsedId))
            {
                return null;
            }

            return await _context.AppUsers.FirstOrDefaultAsync(u => u.AppUserId == parsedId);
        }

        private async Task<string> SaveAvatarAsync(IFormFile file, string category, string prefix)
        {
            var uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "avatars", category);
            Directory.CreateDirectory(uploadDirectory);

            var extension = Path.GetExtension(file.FileName);
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".png", ".jpg", ".jpeg", ".webp", ".gif", ".svg"
            };
            var safeExtension = allowedExtensions.Contains(extension) ? extension : ".png";
            var fileName = $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}{safeExtension}";
            var fullPath = Path.Combine(uploadDirectory, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await file.CopyToAsync(stream);

            return $"/images/avatars/{category}/{fileName}";
        }

        private async Task SignInAsync(AppUser user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.AppUserId.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
            {
                claims.Add(new Claim("avatar_url", user.AvatarUrl));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
                });
        }

        private bool IsGoogleAuthConfigured()
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];

            return !string.IsNullOrWhiteSpace(clientId) &&
                   !string.IsNullOrWhiteSpace(clientSecret);
        }

        private async Task<string> GenerateUniqueUserNameAsync(string? displayName, string email)
        {
            var baseName = (displayName ?? email.Split('@')[0]).Trim();
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = "google-user";
            }

            var sanitized = new string(baseName
                .Where(ch => char.IsLetterOrDigit(ch) || ch == '-' || ch == '_' || ch == '.')
                .ToArray());

            if (string.IsNullOrWhiteSpace(sanitized))
            {
                sanitized = "google-user";
            }

            if (sanitized.Length > 50)
            {
                sanitized = sanitized[..50];
            }

            var candidate = sanitized;
            var suffix = 1;

            while (await _context.AppUsers.AnyAsync(u => u.UserName == candidate))
            {
                var suffixText = suffix.ToString();
                var maxBaseLength = Math.Max(1, 50 - suffixText.Length - 1);
                var trimmedBase = sanitized.Length > maxBaseLength
                    ? sanitized[..maxBaseLength]
                    : sanitized;

                candidate = $"{trimmedBase}_{suffixText}";
                suffix++;
            }

            return candidate;
        }
    }
}
