using System.ComponentModel.DataAnnotations.Schema;

namespace LifeApi.Data.Entities
{
    public class BoardGeneration : Base
    {
        public required Guid BoardId { get; set; }
        public required BoardData Data { get; set; }
        public bool IsLatest { get; set; }

        [ForeignKey("BoardId")]
        public Board? ParentBoard { get; set; }
    }
}