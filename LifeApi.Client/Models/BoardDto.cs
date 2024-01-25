namespace LifeApi.Client.Models
{
    public class BoardDto: BaseDto
    {
        public required string Name { get; set; }
        public required BoardDataDto Data { get; set; }
    }
}