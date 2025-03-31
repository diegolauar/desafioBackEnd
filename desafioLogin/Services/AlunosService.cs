using desafioLogin.Context;
using desafioLogin.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace desafioLogin.Services
{
    public class AlunosService : IAlunoService
    {
        private readonly AppDbContext _context;

        public AlunosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Aluno>> GetAlunos()
        {
            return await _context.Alunos.ToListAsync();
        }

        public async Task<IEnumerable<Aluno>> GetAlunosByNome(string nome)
        {
            return await _context.Alunos
                .Where(n => n.Nome.Contains(nome))
                .ToListAsync();
        }

        public async Task<Aluno> GetAluno(int id)
        {
            return await _context.Alunos.FindAsync(id);
        }

        public async Task CreateAluno(Aluno aluno)
        {
            // Criptografando a senha antes de salvar no banco
            aluno.Password = BCrypt.Net.BCrypt.HashPassword(aluno.Password);

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAluno(Aluno aluno)
        {
            var alunoExistente = await _context.Alunos.FindAsync(aluno.Id);
            if (alunoExistente == null)
                throw new KeyNotFoundException("Aluno não encontrado");

            // Se a senha for alterada, gera um novo hash
            if (!string.IsNullOrEmpty(aluno.Password))
            {
                alunoExistente.Password = BCrypt.Net.BCrypt.HashPassword(aluno.Password);
            }

            alunoExistente.Nome = aluno.Nome; // Atualiza outros dados

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAluno(Aluno aluno)
        {
            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();
        }

    }
}
