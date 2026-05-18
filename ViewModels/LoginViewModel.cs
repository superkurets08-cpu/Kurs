using System.ComponentModel.DataAnnotations;

namespace KURS_ASP.NET.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите логин.")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
