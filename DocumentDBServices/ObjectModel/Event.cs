using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentDBDataService
{
    public class Event
    {
        public string CreatedDate { get; set; }
        public string CampaignId { get; set; }
        public List<string> Followers { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string[] KeyWords { get; set; }
        public string Location { get; set; }

        // This could be something like link to google/outlook calender event.
        public string ExternalEventLink { get; set; }
    }
}