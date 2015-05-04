using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DocumentDBDataService
{
    public class StoryMedia
    {
        public string UserId { get; set; }
        public Stream Data { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
    }
}