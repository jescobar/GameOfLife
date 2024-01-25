using System.Text.Json;
using LifeApi.Client.Models;
using LifeApi.Data;
using LifeApi.Data.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace LifeApi.BusinessLogic.Managers
{
    public class BoardManager : GenericEntityManager<LifeApiDbContext, Board>, IBoardManager
    {

        public BoardManager(LifeApiDbContext apiDbContext) : base(apiDbContext)
        {
        }

        public async Task<BoardDto> Create(BoardDto board)
        {
            var isNew = board.Id.HasValue && board.Id.Value != default;
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var record = await Save(board);

                await ResetLatestGeneration(record.Id!.Value);

                context.BoardGenerations.Add(new BoardGeneration()
                {
                    Id = Guid.NewGuid(),
                    BoardId = record.Id!.Value,
                    Data = record.Data,
                    IsLatest = true,
                });
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return record.Adapt<BoardDto>();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ResetLatestGeneration(Guid boardId)
        {
            await context.BoardGenerations
                .Where(r => r.BoardId == boardId && r.IsLatest == true)
                .ExecuteUpdateAsync(
                s => s
                    .SetProperty(p => p.IsLatest, p => false)
                );
        }

        public async Task<BoardResponseDto> MoveTo(Guid boardId, int iterationsLimit = 1)
        {
            var board = await context.Boards
                .Include(b => b.Generations)
                .SingleOrDefaultAsync(b => b.Id == boardId);

            if (board != null)
            {

                if (board.Data.IsFinalState || board.Data.IsErrorState) {
                    return board.Adapt<BoardResponseDto>();
                }

                var result = board.Data.MoveTo(board.Generations!.Select(x => x.Data).ToList(), iterationsLimit);

                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    await ResetLatestGeneration(boardId);

                    var boardGenerations = result.allIterations.Select((board, i) => new BoardGeneration()
                    {
                        Id = Guid.NewGuid(),
                        BoardId = boardId,
                        Data = board,
                        IsLatest = i == result.allIterations.Count - 1,
                    });

                    board.Data = result.latestIteration;

                    context.BoardGenerations.AddRange(boardGenerations);

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return board.Adapt<BoardResponseDto>();
            }
            else
            {
                throw new ArgumentException($"The '{boardId}' doesn't exists");
            }
        }


    }
}