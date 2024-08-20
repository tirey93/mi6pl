using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using po2tsv_converter.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace po2tsv_converter.Commands
{
    public class ReplaceTexturesCommand
    {
        private readonly TexturesSettings _settings;

        public bool HasErrors { get; set; }

        public ReplaceTexturesCommand(IOptions<TexturesSettings> options)
        {
            _settings = options.Value;
        }

        public void Execute()
        {
            foreach (var filePath in Directory.GetFiles(_settings.TexturePath))
            {
                var file = File.ReadAllText(filePath);

                JObject jsonObject = JObject.Parse(file);

                JObject framesObject = jsonObject["frames"] as JObject;

                var names = framesObject.Properties()
                                           .Select(p => p.Name)
                                           .Where(x => x.EndsWith($"_{_settings.LangFrom}"))
                                           .ToDictionary(x => x, y => y.Replace($"_{_settings.LangFrom}", $"_{_settings.LangTo}"));

                foreach (var name in names)
                {
                    JObject fromObject = jsonObject["frames"][name.Key] as JObject;
                    jsonObject["frames"][name.Value] = fromObject;
                }

                string modifiedJson = jsonObject.ToString(Formatting.Indented);

                File.WriteAllText($"out\\{Path.GetFileName(filePath)}", modifiedJson);
            }
        }
    }
}
