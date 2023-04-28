using Test1.Models;

namespace Test1.ViewModel
{
    public class BooksViewModel :Book
    {
        public List<Shelf> shelfs { get; set; }
        public List<Rack> racks { get; set; }
        public BooksViewModel()
        {
            shelfs = new List<Shelf>(); 
            racks = new List<Rack>();
        }
    }
}
