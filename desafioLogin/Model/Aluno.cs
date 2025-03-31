using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace desafioLogin.Models
{
    public class Aluno
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public bool Active { get; set; } = true;

    }
}
