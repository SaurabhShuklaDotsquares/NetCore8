using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Dto
{
    public class FileUploadDTO
    {
        public long FId { get; set; }
        public string Name { get; set; }
        public string FileOriginalName { get; set; }
        public string FileExtension { get; set; }
        public long EntityId { get; set; }
        public int SectionId { get; set; }
        public Int64 FileSize { get; set; }
        //public IFormFile File { get; set; }
        public string FileStreams { get; set; }
        public string FilePath { get; set; }
        public int ImageOrder { get; set; }
        //public List<FileUploadTitleDTO> Titles { get; set; }
        //public long ForeignId { get; set; }
    }
}
