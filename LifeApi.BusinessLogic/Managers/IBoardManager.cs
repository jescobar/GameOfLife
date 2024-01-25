using LifeApi.Client.Models;
using LifeApi.Data.Entities;

namespace LifeApi.BusinessLogic.Managers
{
    public interface IBoardManager
    {
        Task<BoardDto> Create(BoardDto board);
        Task<BoardResponseDto> MoveTo(Guid boardId, int iterationsLimit = 1);
    }
}