using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.DAL.Data.Entities
{
    public class WebPage:Entity
    {
        public int TimeToLoad { get; set; }
        public int HTML_Size { get; set; }
        [ForeignKey("Page")]
        public int? ParentPageId { get; set; }
        public int? ReferralPageId { get; set; }
        public WebPage Page { get; set; }
    }
}
