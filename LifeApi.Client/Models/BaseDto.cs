using LifeApi.Client.Interfaces;

namespace LifeApi.Client.Models
{
    public class BaseDto: IIdentifiable, IAuditable
    {
        public Guid? Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}