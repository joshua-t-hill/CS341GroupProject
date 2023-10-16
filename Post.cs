using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS341GroupProject
{
    public class Post
    {
        public String Username {  get; set; }
        public String Photo {  get; set; }
        public String Plant { get; set; }
        public String Notes { get; set; }
        public CollectionView Comments { get; set; }
    }
}
