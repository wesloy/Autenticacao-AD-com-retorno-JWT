using System;
using System.ComponentModel.DataAnnotations;


namespace SI_MicroServicos.Model
{
    public class UserProfile
    {
        [Key]
        public Guid ID{ get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserType Type { get; set; }
        [Required]
        public UserAuthType AuthType { get; set; }
        [Required]
        public DateTime LastSuccessfulLogin { get; set; }
        [Required]
        public string Sistema {get; set; }
    }

    public enum UserType
    {
        NormalUser = 0,
        Administrator = 1,
        SuperAdministrator = 2
    }


    public enum UserAuthType
    {
        ActiveDirectory = 0,
        Local = 1
    }

    public class UserAuth
    {
       /// <summary>
       /// Usuario do AD
       /// </summary>
       /// <example>fulanodetal</example>
        [Required]
        public string User { get; set; }
        /// <summary>
        /// Senha do AD
        /// </summary>
        /// <example>123@Algar</example>
        /// <remarks> Esta senha nao fica salva no SI Micro Servicos</remarks>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// Sitema refere-se ao chamador da API
        /// </summary>
        /// <example>Sentinella</example>
        [Required]
        public string Sistema {get; set; }

    }
}
