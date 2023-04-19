using System;
using System.Collections.Generic;
using System.Globalization;

namespace BatteryDischarger
{
    public static class LanguageHandler
    {
        public enum Languages
        {
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
            hu,
            it,
            ja,
            lt,
            lv,
            nl,
            pl,
            pt,
            ro,
            ru,
            sk,
            sl,
            sv,
            zh
        }

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