namespace BloggerPro.Application.DTOs.PostModule
{
    public class ModuleSortOrderDto
    {
        public Guid PostId { get; set; }
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
        public int Index { get; set; }
    }
}
