using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentDBDataService
{
    public enum StoryCategory
    {
        Environmental,
        Social,
        Political,
        Economical,
        Local
    }

    public class Story
    {
        public string CreatedDate { get; set; }
        public string OwnerId { get; set; }
        public string Heading { get; set; }
        public string Category { get; set; }
        public StoryMedia StoryVisualResource { get; set; }
        public string Message { get; set; }
        public bool IsLocal { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string[] KeyWords { get; set; }
    }

    public class DBStory
    {
        public string CreatedDate { get; set; }
        public string OwnerId { get; set; }
        public string Heading { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public bool IsLocal { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string[] KeyWords { get; set; }
        public string StoryMediaResourceBlob { get; set; }
    }
}