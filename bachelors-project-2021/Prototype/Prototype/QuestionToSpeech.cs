using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    /// <summary>
    ///     Class providing functionality to convert text to speech
    /// </summary>
    public class QuestionToSpeech
    {
        private SpeechOptions? settings;

        private readonly Task<SpeechOptions> initTask;

        public QuestionToSpeech()
        {
            initTask = InitializeSettings();
        }

        private async Task<SpeechOptions> InitializeSettings()
        {
            IEnumerable<Locale> locales = await TextToSpeech.Default.GetLocalesAsync();

            // Try to find Finnish locale (fi-FI)
            var finnishLocale = locales.FirstOrDefault(l =>
                l.Language.StartsWith("fi") ||
                l.Name.Contains("Finnish") ||
                l.Name.Contains("Suomi"));

            var locale = finnishLocale ?? locales.FirstOrDefault();

            return new SpeechOptions
            {
                Pitch = 1.0f,
                Volume = 0.5f,
                Locale = locale
            };
        }

        /// <summary>
        ///     Convert text to speech
        /// </summary>
        /// <param name="text">Desired text</param>
        /// <returns></returns>
        public async Task Speak(string text)
        {
            // Wait that settings are available
            settings ??= await initTask;

            await TextToSpeech.Default.SpeakAsync(text, settings);
        }
    }
}
