using Microsoft.Extensions.Options;
using po2tsv_converter.Settings;
using System.Text;

namespace po2tsv_converter.Commands
{
    public class CollectableToPoCommand
    {
        private readonly MainSettings _settings;
        private readonly string[] _polLines;
        private readonly List<string> _engLines;

        public bool HasErrors { get; set; }

        public CollectableToPoCommand(IOptions<MainSettings> options)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _settings = options.Value;

            var errors = string.Empty;
            if (!File.Exists(_settings.EngTsvPath))
                errors += "Error: EngTsvPath was not found in given path\n";
            if (!File.Exists(_settings.PolTsvPath))
                errors += "Error: PolTsvPath was not found in given path\n";
            if (!string.IsNullOrEmpty(errors))
            {
                Console.WriteLine(errors);
                Console.ReadKey();
                HasErrors = true;
                return;
            }

            var linesPol = File.ReadAllLines(_settings.PolTsvPath);
            var linesEng = File.ReadAllLines(_settings.EngTsvPath);

            _polLines = new string[linesEng.Length];
            _engLines = new List<string>();
            int i = 0;
            foreach (var linePl in linesPol)
            {
                var splitted = linePl.Split('\t');
                _polLines[i] = string.Join("\t", splitted[1..]);
                i++;
            }

            foreach (var lineEng in linesEng)
            {
                var splitted = lineEng.Split('\t');
                _engLines.Add(string.Join("\t", splitted[1..]));
            }
        }

        public void Execute()
        {
            var poResult = new StringBuilder();
            int i = 0;
            foreach (var lineEng in _engLines)
            {
                var markup = "COMMON";
                var engStr = lineEng;
                var polStr = _polLines[i];
                poResult.Append(ToPo(markup, engStr, polStr));
                i++;
            }

            File.WriteAllText(_settings.PoFilePath, poResult.ToString());
        }

        private static string ToPo(string markup, string engStr, string plStr)
        {
            var result = $"msgctxt \"{markup}\"\n";
            result += $"msgid \"{engStr}\"\n";
            result += $"msgstr \"{plStr}\"\n\n";

            return result;
        }
    }
}
