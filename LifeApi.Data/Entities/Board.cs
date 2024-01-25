namespace LifeApi.Data.Entities;

public class Board: Base
{
    public required string Name { get; set; }
    public required BoardData Data { get; set; }
    public ICollection<BoardGeneration>? Generations { get; set; }
}