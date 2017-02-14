using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HTML_Parser.DAL.Data.Entities
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        public string URL { get; set; }
    }
}
