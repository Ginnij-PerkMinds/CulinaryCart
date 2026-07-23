namespace CulinaryCartBAL.Models.DTO
{
    public class CategoryUpdateRequest
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

