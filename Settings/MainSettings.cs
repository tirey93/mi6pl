using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace po2tsv_converter.Settings
{
    public class MainSettings
    {
        public Mode Mode { get; set; }
        public string PoFilePath { get; set; }
        public string EngTsvPath { get; set; }
        public string PolTsvPath { get; set; }
        public string LangId { get; set; }
    }

    public enum Mode
    {
        ToPo,
        FromPo
    }
}
