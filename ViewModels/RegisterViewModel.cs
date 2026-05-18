using System.ComponentModel.DataAnnotations;

namespace KURS_ASP.NET.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите логин.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен содержать от 3 до 50 символов.")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Электронная почта")]
        [Required(ErrorMessage = "Введите электронную почту.")]
        [EmailAddress(ErrorMessage = "Введите корректную электронную почту.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите пароль.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать не меньше 6 символов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Подтверждение пароля")]
        [Required(ErrorMessage = "Подтвердите пароль.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
