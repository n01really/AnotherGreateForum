namespace Another_Great_Forum.Models
{
    public class Forum
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
    }
}
