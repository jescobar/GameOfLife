namespace LifeApi.Client.Models
{
    public class BoardDataResponseDto
    {
        public required List<List<bool>> Matrix { get; set; }
        public int ActiveCells { get; private set; }
        public int InactiveCells { get; private set; }
        public bool IsFinalState { get; set; }
        public bool IsErrorState { get; set; }
    }
}