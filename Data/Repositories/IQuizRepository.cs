using QuizMaker.Models;
using System.Threading.Tasks;

namespace QuizMaker.Data.Repositories
{
    public interface IQuizRepository : IGenericRepository<Quiz>
    {
        Task<Quiz?> GetWithQuestionsAsync(int id); 
    }
}
