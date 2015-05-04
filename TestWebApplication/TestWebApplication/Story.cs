using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWebApplication
{
    public enum StoryCategory
    {
        Environmental,
        Social,
        Political,
        Economical,
        Local
    }

    public class StoryMedia
    {
        public string UserId { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
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