using Microsoft.Extensions.Options;
using po2tsv_converter.Settings;
using System.Text;

namespace po2tsv_converter.Commands
{
    public class ToPoCommand
    {
        private readonly MainSettings _settings;
        private readonly Dictionary<string, string> _polLines;
        private readonly Dictionary<string, string> _engLines;

        public bool HasErrors { get; set; }

        public ToPoCommand(IOptions<MainSettings> options)
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
                HasErrors = true;
                return;
            }

            var linesPol = File.ReadAllLines(_settings.PolTsvPath);
            var linesEng = File.ReadAllLines(_settings.EngTsvPath);

            _polLines = new Dictionary<string, string>();
            _engLines = new Dictionary<string, string>();
            foreach (var linePl in linesPol[1..])
            {
                var splitted = linePl.Split('\t');
                _polLines.Add($"{splitted[1]}_{splitted[0]}", splitted[2]);
            }

            foreach (var lineEng in linesEng[1..])
            {
                var splitted = lineEng.Split('\t');
                _engLines.Add($"{splitted[1]}_{splitted[0]}", splitted[2]);
            }
        }

        public void Execute()
        {
            var poResult = new StringBuilder();
            foreach (var lineEng in _engLines)
            {
                var markup = lineEng.Key;
                var engStr = lineEng.Value;
                var polStr = _polLines[markup];
                poResult.Append(ToPo(markup, engStr, polStr));
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
