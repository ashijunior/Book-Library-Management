namespace BookManagement.Models
{
    public class Authors
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }

        public ICollection<Books> Books { get; set; }
    }
}
