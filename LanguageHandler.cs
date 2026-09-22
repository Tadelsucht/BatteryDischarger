using System;
using System.Collections.Generic;
using System.Globalization;

namespace BatteryDischarger
{
    public static class LanguageHandler
    {
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
