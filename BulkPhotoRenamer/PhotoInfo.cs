using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkPhotoRenamer
{
    class PhotoInfo
    {
        public PhotoInfo (string filePath, string prefix, bool addTimestamp)
        {
            this.filePath = filePath;
            this.prefix = prefix;
            this.addTimestamp = addTimestamp;
        }

        public string filePath { get; set; }
        public string prefix { get; set; }
        public bool addTimestamp { get; set; }

        public string originalFileName { get; set; }
        public DateTime dateTime { get; set; }
        public enum dtType { Taken = 1, Created = 2 }
        public dtType dateTimeType { get; set; }
        public string baseFileName { get; set; }
        public string newFileName { get; set; }
    }
}
