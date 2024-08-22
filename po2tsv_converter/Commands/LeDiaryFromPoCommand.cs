using Microsoft.Extensions.Options;
using po2tsv_converter.Settings;
using po2tsv_converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace po2tsv_converter.Commands
{
    public class LeDiaryFromPoCommand
    {
        private readonly MainSettings _settings;
        private readonly List<string> _polLines;

        public bool HasErrors { get; set; }

        public LeDiaryFromPoCommand(IOptions<MainSettings> options)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _settings = options.Value;

            var errors = string.Empty;
            if (!File.Exists(_settings.PoFilePath))
                errors += "Error: PoFilePath was not found in given path\n";
            if (!string.IsNullOrEmpty(errors))
            {
                Console.WriteLine(errors);
                Console.ReadKey();
                HasErrors = true;
                return;
            }
            var linesEng = File.ReadAllLines(_settings.EngTsvPath);
            string file = File.ReadAllText(_settings.PoFilePath);
            var splitted = file.Split("msgctxt");


            _polLines = new List<string>();

            foreach (var text in splitted)
            {
                if (string.IsNullOrEmpty(text)) continue;
                var textWithCuttedStart = "msgctxt" + text;

                var splitter = new PoSplitter(textWithCuttedStart);
                if (!splitter.IsValid)
                    continue;

                var plText = splitter.PlText;

                _polLines.Add(plText);
            }
        }
        
        public void Execute() 
        {
            var result = new StringBuilder();

            foreach (var engLine in _polLines)
            {
                result.AppendLine(engLine);
            }

            File.WriteAllText(_settings.PolTsvPath, result.ToString());
        }
    }
}
