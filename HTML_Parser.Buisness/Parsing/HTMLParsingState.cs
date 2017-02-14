using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;

namespace HTML_Parser.Business.Parsing
{
    public class HTMLParsingState
    {
        string url;
        int depth = -2;
        int? parentPageId;
        int? referralPageId;

        public HTMLParsingState(string url,int?parentPageId,int?referralPageId, int depth = -2)
        {
            this.url = url;
            this.depth = depth;
            this.parentPageId = parentPageId;
            this.referralPageId = referralPageId;
        }
        public string GetUrl()
        {
            return url;
        }
        public int GetDepth()
        {
            return depth;
        }
        public void IncrementDepth()
        {
            if (depth > -1) depth--;
        }
        public int? GetParentPageId()
        {
            return parentPageId;
        }
        public int? GetReferralPageId()
        {
            return referralPageId;
        }
    }
}
