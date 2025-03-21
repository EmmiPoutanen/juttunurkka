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
        private SpeechOptions settings = new() { };

        public QuestionToSpeech()
        {
            InitializeSettings();
        }

        private async void InitializeSettings()
        {
            IEnumerable<Locale> locales = await TextToSpeech.Default.GetLocalesAsync();

            // Try to find Finnish locale (fi-FI)
            var finnishLocale = locales.FirstOrDefault(l =>
                l.Language.StartsWith("fi") ||
                l.Name.Contains("Finnish") ||
                l.Name.Contains("Suomi"));

            settings = new SpeechOptions
            {
                Pitch = 1.0f,
                Volume = 0.5f,
                Locale = finnishLocale // Will be null if Finnish isn't found
            };
        }

        /// <summary>
        ///     Convert text to speech
        /// </summary>
        /// <param name="text">Desired text</param>
        /// <returns></returns>
        public async Task Speak(string text)
        {
            await TextToSpeech.Default.SpeakAsync(text, settings);
        }
    }
}
