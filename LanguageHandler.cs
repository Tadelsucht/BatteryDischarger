using System;
using System.Collections.Generic;
using System.Globalization;

namespace BatteryDischarger
{
    // Defines the language codes that the UI can select and resolve through .NET cultures.
    public static class LanguageHandler
    {
        // Keep these ISO language codes aligned with the available UI resource files.
        public enum Languages
        {
            ar,
            bg,
            cs,
            da,
            de,
            el,
            en,
            es,
            et,
            fi,
            fr,
            he,
            hu,
            id,
            it,
            ja,
            ko,
            lt,
            lv,
            nb,
            nl,
            pl,
            pt,
            ro,
            ru,
            sk,
            sl,
            sv,
            tr,
            uk,
            vi,
            zh
        }

        // Returns stable language-code prefixes together with the display names from the current culture data.
        public static IEnumerable<string> GetLanguages()
        {
            foreach (Languages entry in Enum.GetValues(typeof(Languages)))
            {
                var twoLetterLanguageCode = Enum.GetName(typeof(Languages), entry);
                yield return twoLetterLanguageCode + ": " + (new CultureInfo(twoLetterLanguageCode).DisplayName);
            }
        }
    }
}
