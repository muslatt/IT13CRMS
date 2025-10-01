using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateCRMWinForms.Models
{
    public class PropertyProofFile
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public long FileSize { get; set; }

        // Navigation property
        public Property Property { get; set; } = null!;
    }
}