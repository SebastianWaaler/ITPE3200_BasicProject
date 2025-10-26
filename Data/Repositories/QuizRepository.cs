using Microsoft.EntityFrameworkCore;
using QuizMaker.Models;

namespace QuizMaker.Data.Repositories;

public class QuizRepository(ApplicationDbContext ctx) : GenericRepository<Quiz>(ctx), IQuizRepository
{
    private readonly ApplicationDbContext _ctx = ctx;

    public async Task<Quiz?> GetWithQuestionsAsync(int id)

    {
        return await _ctx.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);
    }
}
