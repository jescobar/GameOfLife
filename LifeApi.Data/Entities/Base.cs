using System.ComponentModel.DataAnnotations;
using LifeApi.Client.Interfaces;

namespace LifeApi.Data.Entities
{
    public abstract class Base: IAuditable, IIdentifiable
    {
        [Key]
        public Guid? Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
    }
}