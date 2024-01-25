namespace LifeApi.Client.Models
{
    public class BoardResponseDto: BaseDto
    {
        public required string Name { get; set; }
        public required BoardDataResponseDto Data { get; set; }
    }
}