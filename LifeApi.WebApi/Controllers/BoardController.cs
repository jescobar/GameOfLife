using LifeApi.BusinessLogic.Managers;
using LifeApi.Client.Models;
using LifeApi.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LifeApi.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardManager boardManager;

        public BoardController(IBoardManager boardManager)
        {
            this.boardManager = boardManager;

        }
        [HttpPost]
        public Task<BoardDto> Upsert(BoardDto input)
        {
            return boardManager.Create(input);
        }

        [HttpPost]
        [Route("{boardId}/next")]
        public Task<BoardResponseDto> NextState(Guid boardId)
        {
            return boardManager.MoveTo(boardId);
        }

        [HttpPost]
        [Route("{boardId}/calculate/{steps}")]
        public Task<BoardResponseDto> CalculateState(Guid boardId, int steps)
        {
            return boardManager.MoveTo(boardId, steps);
        }

        [HttpPost]
        [Route("{boardId}/final")]
        public Task<BoardResponseDto> CalculateState(Guid boardId)
        {
            return boardManager.MoveTo(boardId, 100);
        }
    }
}