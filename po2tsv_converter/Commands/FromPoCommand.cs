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
    public class FromPoCommand
    {
        private readonly MainSettings _settings;
        private readonly List<string> _engLines;
        private readonly Dictionary<string, string> _polLines;

        public bool HasErrors { get; set; }

        public FromPoCommand(IOptions<MainSettings> options)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _settings = options.Value;

            var errors = string.Empty;
            if (!File.Exists(_settings.EngTsvPath))
                errors += "Error: EngTsvPath was not found in given path\n";
            if (!File.Exists(_settings.PoFilePath))
                errors += "Error: PoFilePath was not found in given path\n";
            if (!string.IsNullOrEmpty(errors))
            {
                Console.WriteLine(errors);
                HasErrors = true;
                return;
            }
            var linesEng = File.ReadAllLines(_settings.EngTsvPath);
            string file = File.ReadAllText(_settings.PoFilePath);
            var splitted = file.Split("msgctxt");

            _engLines = [..linesEng];

            _polLines = new Dictionary<string, string>();

            foreach (var text in splitted)
            {
                if (string.IsNullOrEmpty(text)) continue;
                var textWithCuttedStart = "msgctxt" + text;

                var splitter = new PoSplitter(textWithCuttedStart);
                if (!splitter.IsValid)
                    continue;

                var markup = splitter.Markup;
                var plText = splitter.PlText;

                _polLines.Add(markup, plText);
            }
        }
        
        public void Execute() 
        {
            var result = new StringBuilder();

            var firstLineSplitted = _engLines[0].Split("\t");
            if (!string.IsNullOrEmpty(_settings.LangId))
                firstLineSplitted[2] = _settings.LangId;
            result.AppendLine(string.Join("\t", firstLineSplitted));

            foreach (var engLine in _engLines[1..])
            {
                var splitted = engLine.Split('\t');
                splitted[2] = _polLines[$"{splitted[1]}_{splitted[0]}"];

                result.AppendLine(string.Join("\t", splitted));
            }

            File.WriteAllText(_settings.PolTsvPath, result.ToString());
        }
    }
}
