namespace pos_system.pos.Models
{
    public class SalesFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
    }
}