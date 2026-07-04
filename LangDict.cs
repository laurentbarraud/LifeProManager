/// <file>LangDict.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LifeProManager
{
    /// <summary>
    /// Central repository for multilingual keyword dictionaries used by SmartSearch.
    /// </summary>
    public static class LangDict
    {
        // ------------------------------------------------------------
        // User-editable keyword lists.
        // New keywords for additional languages can be inserted here.
        // ------------------------------------------------------------

        /// Defines multi-token backward-direction expressions equivalent to "ago".
        /// These structures are language-specific.
        internal static readonly List<string[]> AgoMultiTokenStructures = new List<string[]>
        {
            new string[] { "il", "y", "a" },   // French
            new string[] { "hace" }            // Spanish
        };

        // Keywords that separate the left and right segments of a range expression.
        // Examples: "2 weeks and 3 days", "2 semaines et 3 jours", "2 semanas y 3 dias".
        internal static readonly (string key, string value)[] lstAndKeywords =
        {
            ("fr", "et"),
            ("en", "and"),
            ("es", "y")
        };

        // Keywords that introduce a "between X and Y" range expression.
        internal static readonly (string key, string value)[] lstBetweenKeywords =
        {
            ("fr", "entre"),
            ("en", "between"),
            ("es", "entre")
        };

        // Prepositions used in absolute date expressions ("2nd of April", "2 de abril", "le 2 mars")
        internal static readonly string[] lstDatePrepositions =
        {
            // English
            "of",

            // French
            "de",
            "le",

            // Spanish
            "de"
        };

        // Day words used in ordinal expressions ("3rd day", "7eme jour", "2do dia").
        internal static readonly string[] lstDayKeywords =
        {
            // English
            "day", "days",

            // French
            "jour", "jours",
            "journée", "journee",
            "journées", "journees",

            // Spanish
            "dia", "día", "dias", "días"
        };

        // Ignored keywords (articles, determiners, non-semantic words)
        // Used to clean multi-token expressions before collapsing.
        internal static readonly (string key, string value)[] lstIgnoredKeywords =
        {
            ("the", "the"),
            ("le", "le"),
            ("la", "la"),
            ("les", "les"),
            ("un", "un"),
            ("une", "une"),
            ("des", "des"),
            ("el", "el"),
            ("lo", "lo"),
            ("los", "los"),
            ("las", "las")
        };

        // Month names mapped to their numeric value (1–12).
        // Includes full names and common variants across supported vocabularies.
        internal static readonly (string key, int value)[] lstMonthNames =
        {
            ("january", 1), ("janvier", 1), ("enero", 1),

            ("february", 2), ("fevrier", 2), ("febrero", 2),

            ("march", 3), ("mars", 3), ("marzo", 3),

            ("april", 4), ("avril", 4), ("abril", 4),

            ("may", 5), ("mai", 5), ("mayo", 5),

            ("june", 6), ("juin", 6), ("junio", 6),

            ("july", 7), ("juillet", 7), ("julio", 7),

            ("august", 8), ("aout", 8), ("agosto", 8),

            ("september", 9), ("septembre", 9), ("septiembre", 9),

            ("october", 10), ("octobre", 10), ("octubre", 10),

            ("november", 11), ("novembre", 11), ("noviembre", 11),

            ("december", 12), ("decembre", 12), ("diciembre", 12)
        };

        // Month‑range expressions ("this month", "next month", "last month") in all supported languages.
        internal static readonly (string key, string value)[] lstMonthRangeKeywords =
        {
            // This month
            ("this month", "this"),
            ("current month", "this"),
            ("thismonth", "this"),
            ("ce mois", "this"),
            ("ce mois ci", "this"),
            ("ce mois-ci", "this"),
            ("mois courant", "this"),
            ("mois actuel", "this"),
            ("mois en cours", "this"),
            ("este mes", "this"),
            ("mes actual", "this"),
            ("mes en curso", "this"),

            // Next month
            ("next month", "next"),
            ("following month", "next"),
            ("the month after", "next"),
            ("mois prochain", "next"),
            ("mois suivant", "next"),
            ("mois d apres", "next"),
            ("mois d'après", "next"),
            ("mes proximo", "next"),
            ("mes siguiente", "next"),
            ("el mes que viene", "next"),

            // Last month
            ("last month", "last"),
            ("previous month", "last"),
            ("the month before", "last"),
            ("mois d'avant", "last"),
            ("mois avant", "last"),
            ("mois dernier", "last"),
            ("mois passe", "last"),
            ("mois précédent", "last"),
            ("mes pasado", "last"),
            ("mes anterior", "last")
        };

        // Month words used in relative expressions (e.g. "next month").
        internal static readonly string[] lstMonthKeywords =
        {
            "month", "months", "mois", "mes", "mese"
        };

        internal static readonly (string key, int value)[] lstNumberMultipliers =
        {
            // French
            ("cent", 100), ("cents", 100), ("centaine", 100),
            ("mille", 1000),

            // English
            ("hundred", 100), ("hundreds", 100),
            ("thousand", 1000),

            // Spanish
            ("cien", 100), ("ciento", 100),
            ("mil", 1000)
        };

        internal static readonly (string key, int value)[] lstNumberTens =
        {
            // French
            ("dix", 10), ("onze", 11), ("douze", 12), ("treize", 13),
            ("quatorze", 14), ("quinze", 15), ("seize", 16),
            ("vingt", 20), ("trente", 30), ("quarante", 40),
            ("cinquante", 50), ("soixante", 60),
            ("septante", 70), ("huitante", 80), ("nonante", 90),
            ("quatrevingt", 80), ("quatre-vingt", 80), ("quatre vingt", 80),

            // English
            ("ten", 10), ("eleven", 11), ("twelve", 12),
            ("twenty", 20), ("thirty", 30), ("forty", 40),
            ("fifty", 50), ("sixty", 60), ("seventy", 70),
            ("eighty", 80), ("ninety", 90),

            // Spanish
            ("diez", 10), ("once", 11), ("doce", 12), ("trece", 13),
            ("catorce", 14), ("quince", 15), ("dieciseis", 16),
            ("veinte", 20), ("treinta", 30), ("cuarenta", 40),
            ("cincuenta", 50), ("sesenta", 60), ("setenta", 70),
            ("ochenta", 80), ("noventa", 90)
        };

        internal static readonly (string key, int value)[] lstNumberUnits =
        {
            // French
            ("zero", 0), ("zéro", 0),
            ("un", 1), ("une", 1),
            ("deux", 2), ("trois", 3), ("quatre", 4),
            ("cinq", 5), ("six", 6), ("sept", 7),
            ("huit", 8), ("neuf", 9),

            // English
            ("zero", 0),
            ("one", 1), ("two", 2), ("three", 3),
            ("four", 4), ("five", 5), ("six", 6),
            ("seven", 7), ("eight", 8), ("nine", 9),

            // Spanish
            ("cero", 0),
            ("uno", 1), ("una", 1),
            ("dos", 2), ("tres", 3), ("cuatro", 4),
            ("cinco", 5), ("seis", 6), ("siete", 7),
            ("ocho", 8), ("nueve", 9)
        };

        // Ordinal suffixes
        internal static readonly string[] lstOrdinalSuffixes =
        {
            "er", "eme", "ème", "e",
            "st", "nd", "rd", "th",
            "ro", "do", "to"
        };

        // Priority and category keywords
        internal static readonly (string key, string value)[] lstPriorities =
        {
            // Important
            ("important", "important"), ("importante", "important"),
            ("importantes", "important"), ("urgent", "important"),
            ("urgente", "important"), ("urgentes", "important"),
            ("essential", "important"), ("essentiel", "important"),
            ("essentielle", "important"), ("essentielles", "important"),
            ("essentials", "important"), ("critical", "important"),
            ("critique", "important"), ("crucial", "important"),
            ("priority", "important"), ("priorite", "important"),
            ("priorité", "important"), ("high", "important"),

            // Birthdays
            ("anniversaire", "anniversary"), ("anniversaires", "anniversary"),
            ("anniv", "anniversary"), ("anni", "anniversary"),
            ("fete", "anniversary"), ("fêtes", "anniversary"),
            ("anniversary", "anniversary"), ("birthday", "anniversary"),
            ("birthdays", "anniversary"), ("bday", "anniversary"),
            ("b-day", "anniversary"), ("cumpleanos", "anniversary"),
            ("cumpleaños", "anniversary")
        };

        // Optional prefixes that may appear before a numeric range ("from", "du", "del", etc.)
        internal static readonly string[] lstRangeOptionalPrefixes =
        {
            // English
            "from", "starting", "start", "beginning", "period",

            // French
            "du", "de", "depuis", "période", "periode", "à", "a", "partir",

            // Spanish
            "del", "desde", "periodo", "a", "a partir", "a partir de"
        };

        // Range separators ("to", "au", "al", "-", etc.)
        internal static readonly string[] lstRangeSeparators =
        {
            // English
            "to", "until", "till", "-",

            // French
            "au", "à", "a", "jusqu", "jusquau", "jusqu'au",

            // Spanish
            "al", "hasta"
        };

        internal static readonly (string key, int value)[] lstRelativeDayOffsets =
        {
            ("today", 0), ("aujourdhui", 0), ("hoy", 0),
            ("tomorrow", 1), ("demain", 1), ("manana", 1),
            ("yesterday", -1), ("hier", -1), ("ayer", -1),
            ("dayaftertomorrow", 2), ("apresdemain", 2), ("pasadomanana", 2),
            ("daybeforeyesterday", -2), ("avanthier", -2), ("anteayer", -2)
        };


        internal static readonly string[] lstShowAllKeywords =
        {
            "*",
            "all",
            "todo",
            "todos",
            "toutes",
            "tous",
            "toute",
            "alles",
            "alle",
            "tout"
        };

        internal static readonly (string key, int value)[] lstTemporalDirections =
        {
            // Backward
            ("last", -1),
            ("previous", -1),
            ("ago", -1),

            ("dernier", -1),
            ("dernière", -1),
            ("derniers", -1),
            ("dernières", -1),
            ("passe", -1),
            ("passé", -1),
            ("passée", -1),
            ("precedent", -1),
            ("précédent", -1),
            ("précédente", -1),

            ("anterior", -1),
            ("pasado", -1),
            ("pasada", -1),
            ("pasados", -1),
            ("pasadas", -1),
            ("hace", -1),

            // Neutral ("this")
            ("this", 0),
            ("current", 0),
            ("thisweek", 0),
            ("thismonth", 0),
            ("thisyear", 0),

            ("ce", 0),
            ("cet", 0),
            ("cette", 0),
            ("ces", 0),
            ("courant", 0),
            ("courante", 0),
            ("actuel", 0),
            ("actuelle", 0),
            ("en cours", 0),
            ("encours", 0),

            ("este", 0),
            ("esta", 0),
            ("estos", 0),
            ("estas", 0),
            ("actual", 0),
            ("en curso", 0),
            ("encurso", 0),

            // Forward
            ("next", +1),
            ("upcoming", +1),

            ("prochain", +1),
            ("prochaine", +1),
            ("prochains", +1),
            ("prochaines", +1),

            ("siguiente", +1),
            ("proximo", +1),
            ("próximo", +1),
            ("proxima", +1),
            ("próxima", +1),
            ("proximos", +1),
            ("próximos", +1),
            ("proximas", +1),
            ("próximas", +1)
        };

        internal static readonly string[] lstTemporalPrepositions =
        {
            "in", "within", "dans", "en", "dentro", "dentro de"
        };

        internal static readonly (string key, string value)[] lstTemporalUnits =
        {
            // English
            ("day", "day"), ("days", "day"),
            ("week", "week"), ("weeks", "week"),
            ("month", "month"), ("months", "month"),
            ("year", "year"), ("years", "year"),

            // French
            ("jour", "day"), ("jours", "day"),
            ("journée", "day"), ("journee", "day"),
            ("journées", "day"), ("journees", "day"),

            ("semaine", "week"), ("semaines", "week"),
            ("mois", "month"),

            ("annee", "year"), ("année", "year"),
            ("annees", "year"), ("années", "year"),

            // Spanish
            ("dia", "day"), ("día", "day"),
            ("dias", "day"), ("días", "day"),

            ("semana", "week"), ("semanas", "week"),

            ("mes", "month"), ("meses", "month"),

            ("año", "year"), ("anos", "year"), ("años", "year")
        };

        internal static readonly (string key, int value)[] lstTemporalUnitToDays =
        {
            ("day", 1), ("jour", 1), ("jours", 1), ("dia", 1), ("día", 1),
            ("week", 7), ("semaine", 7), ("semana", 7),
            ("month", 30), ("mois", 30), ("mes", 30),
            ("year", 365), ("annee", 365), ("année", 365), ("ano", 365), ("año", 365)
        };

        internal static readonly (string key, DayOfWeek value)[] lstWeekdays =
        {
            ("monday", DayOfWeek.Monday), ("mon", DayOfWeek.Monday),
            ("tuesday", DayOfWeek.Tuesday), ("tue", DayOfWeek.Tuesday),
            ("wednesday", DayOfWeek.Wednesday), ("wed", DayOfWeek.Wednesday),
            ("thursday", DayOfWeek.Thursday), ("thu", DayOfWeek.Thursday),
            ("friday", DayOfWeek.Friday), ("fri", DayOfWeek.Friday),
            ("saturday", DayOfWeek.Saturday), ("sat", DayOfWeek.Saturday),
            ("sunday", DayOfWeek.Sunday), ("sun", DayOfWeek.Sunday),

            ("lundi", DayOfWeek.Monday),
            ("mardi", DayOfWeek.Tuesday),
            ("mercredi", DayOfWeek.Wednesday),
            ("jeudi", DayOfWeek.Thursday),
            ("vendredi", DayOfWeek.Friday),
            ("samedi", DayOfWeek.Saturday),
            ("dimanche", DayOfWeek.Sunday),

            ("lunes", DayOfWeek.Monday),
            ("martes", DayOfWeek.Tuesday),
            ("miercoles", DayOfWeek.Wednesday), ("miércoles", DayOfWeek.Wednesday),
            ("jueves", DayOfWeek.Thursday),
            ("viernes", DayOfWeek.Friday),
            ("sabado", DayOfWeek.Saturday), ("sábado", DayOfWeek.Saturday),
            ("domingo", DayOfWeek.Sunday)
        };

        // Week words used in relative expressions ("next week").
        internal static readonly string[] lstWeekKeywords =
        {
            "week", "weeks", "semaine", "semaines", "semana", "semanas"
        };

        // Week‑range expressions ("this week", "next week", "last week") in all supported languages.
        internal static readonly (string key, string value)[] lstWeekRangeKeywords =
        {
            // This week
            ("this week", "this"),
            ("current week", "this"),
            ("thisweek", "this"),
            ("semaine en cours", "this"),
            ("semaine actuelle", "this"),
            ("esta semana", "this"),
            ("semana actual", "this"),
            ("semana en curso", "this"),

            // Next week
            ("next week", "next"),
            ("following week", "next"),
            ("the week after", "next"),
            ("semaine prochaine", "next"),
            ("semaine suivante", "next"),
            ("semaine apres", "next"),
            ("semaine après", "next"),
            ("la semana que viene", "next"),
            ("semana proxima", "next"),
            ("semana próxima", "next"),
            ("semana siguiente", "next"),

            // Last week
            ("last week", "last"),
            ("previous week", "last"),
            ("the week before", "last"),
            ("semaine derniere", "last"),
            ("semaine dernière", "last"),
            ("semaine passee", "last"),
            ("semaine passée", "last"),
            ("semaine precedente", "last"),
            ("semaine précédente", "last"),
            ("semaine avant", "last"),
            ("semaine d'avant", "last"),
            ("la semana pasada", "last"),
            ("semana pasada", "last"),
            ("semana anterior", "last")
        };

        // Year words used in relative expressions ("next year", "année prochaine", "año próximo").
        internal static readonly string[] lstYearKeywords =
        {
            // English
            "year", "years",

            // French (with and without accents)
            "année", "annee",
            "années", "annees",
            "an", "ans",

            // Spanish (with and without accents)
            "año", "ano",
            "años", "anos"
        };

        // Year‑range expressions ("this year", "next year", "last year") in all supported languages.
        internal static readonly (string key, string value)[] lstYearRangeKeywords =
        {
            // This year
            ("this year", "this"),
            ("current year", "this"),
            ("thisyear", "this"),
            ("cette annee", "this"),
            ("cette année", "this"),
            ("annee en cours", "this"),
            ("année en cours", "this"),
            ("annee actuelle", "this"),
            ("année actuelle", "this"),
            ("annee courante", "this"),
            ("année courante", "this"),
            ("este ano", "this"),
            ("este año", "this"),
            ("ano actual", "this"),
            ("año actual", "this"),
            ("ano en curso", "this"),
            ("año en curso", "this"),

            // Next year
            ("next year", "next"),
            ("the year after", "next"),
            ("annee prochaine", "next"),
            ("année prochaine", "next"),
            ("prochaine annee", "next"),
            ("prochaine année", "next"),
            ("el ano que viene", "next"),
            ("el año que viene", "next"),
            ("ano proximo", "next"),
            ("año próximo", "next"),
            ("ano siguiente", "next"),
            ("año siguiente", "next"),

            // Last year
            ("last year", "last"),
            ("previous year", "last"),
            ("the year before", "last"),
            ("annee derniere", "last"),
            ("année dernière", "last"),
            ("annee passee", "last"),
            ("année passée", "last"),
            ("el ano pasado", "last"),
            ("el año pasado", "last"),
            ("ano pasado", "last"),
            ("año pasado", "last"),
            ("ano anterior", "last"),
            ("año anterior", "last")
        };

        // ----------------------------------------------------------------
        // Normalized dictionaries and sets
        // Do not modify this section, unless you know what you're doing.
        // ----------------------------------------------------------------

        internal static readonly HashSet<string> AndKeywordSet =
            BuildNormalizedHashSet(lstAndKeywords.Select(x => x.value));

        internal static readonly HashSet<string> DatePrepositionSet =
            BuildNormalizedHashSet(lstDatePrepositions);

        internal static readonly HashSet<string> DayKeywordSet =
            BuildNormalizedHashSet(lstDayKeywords);

        internal static readonly HashSet<string> IgnoredKeywordSet =
            BuildNormalizedHashSet(lstIgnoredKeywords.Select(x => x.value));

        internal static readonly HashSet<string> MonthKeywordSet =
            BuildNormalizedHashSet(lstMonthKeywords);

        internal static readonly Dictionary<string, int> MonthNumberDict =
           lstMonthNames.ToDictionary(monthName => monthName.key, monthName => monthName.value);

        internal static readonly Dictionary<string, string> MonthRangeDict =
            lstMonthRangeKeywords.ToDictionary(monthRange => monthRange.key, monthRange => monthRange.value);

        internal static readonly Dictionary<string, int> NumberMultiplierDict =
            BuildNormalizedDictionary(lstNumberMultipliers);

        internal static readonly Dictionary<string, int> NumberTenDict =
        BuildNormalizedDictionary(lstNumberTens);

        internal static readonly Dictionary<string, int> NumberUnitDict =
           BuildNormalizedDictionary(lstNumberUnits);

        internal static readonly HashSet<string> OrdinalSuffixSet =
            BuildNormalizedHashSet(lstOrdinalSuffixes);

        internal static readonly Dictionary<string, string> PriorityKeywordDict =
            BuildNormalizedDictionary(lstPriorities);

        internal static readonly HashSet<string> RangeOptionalPrefixSet =
           BuildNormalizedHashSet(lstRangeOptionalPrefixes);

        internal static readonly HashSet<string> RangeSeparatorSet =
            BuildNormalizedHashSet(lstRangeSeparators);
        
        internal static readonly Dictionary<string, int> RelativeDayOffsetDict =
            lstRelativeDayOffsets.ToDictionary(relOffset => relOffset.key, relOffset => relOffset.value);

        internal static readonly HashSet<string> ShowAllKeywords =
            BuildNormalizedHashSet(lstShowAllKeywords);

        internal static readonly Dictionary<string, int> TemporalDirectionDict =
            lstTemporalDirections.ToDictionary(tempDirection => tempDirection.key, tempDirection => tempDirection.value);

        internal static readonly HashSet<string> TemporalPrepositionSet =
        BuildNormalizedHashSet(lstTemporalPrepositions);

        internal static readonly Dictionary<string, string> TemporalUnitDict =
            BuildNormalizedDictionary(lstTemporalUnits);

        internal static readonly Dictionary<string, int> TemporalUnitToDays =
            BuildNormalizedDictionary(lstTemporalUnitToDays);

        internal static readonly Dictionary<string, DayOfWeek> WeekdayDict =
            lstWeekdays.ToDictionary(weekDay => weekDay.key, weekDay => weekDay.value);

        internal static readonly Dictionary<string, DayOfWeek> WeekdayNameToDayOfWeekDict =
            WeekdayDict.ToDictionary(
                element => element.Key,
                element => element.Value,
                StringComparer.OrdinalIgnoreCase);

        internal static readonly HashSet<string> WeekKeywordSet =
            BuildNormalizedHashSet(lstWeekKeywords);

        internal static readonly Dictionary<string, string> WeekRangeDict =
            lstWeekRangeKeywords.ToDictionary(weekRange => weekRange.key, weekRange => weekRange.value);

        internal static readonly HashSet<string> YearKeywordSet =
            BuildNormalizedHashSet(lstYearKeywords);

        internal static readonly Dictionary<string, string> YearRangeDict =
            lstYearRangeKeywords.ToDictionary(yearRange => yearRange.key, yearRange => yearRange.value);

        // -----------------------------------------------------------------------------
        // Key normalization helpers
        // These functions take raw data (strings or tuples) and convert them into
        // normalized, lookup‑friendly collections.
        // Do not modify unless you fully understand how normalization works.
        // -----------------------------------------------------------------------------

        private static Dictionary<string, TValue> BuildNormalizedDictionary<TValue>(IEnumerable<(string key, TValue value)> source)
        {
            var groupEntries = source.GroupBy(entry => NormalizeKey(entry.key));

            var normalizedDictionary = groupEntries.ToDictionary(
                group => group.Key,
                group => group.First().value,
                StringComparer.OrdinalIgnoreCase
            );

            return normalizedDictionary;
        }

        private static HashSet<string> BuildNormalizedHashSet(IEnumerable<string> source)
        {
            return source
                .Select(x => NormalizeKey(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Normalizes any input keyword into a canonical, comparable form.
        /// This makes tokens like "28th", "3rd", "1er", "2ème", "3º" all normalize
        /// to their numeric base ("28", "3", "1", "2", "3").
        /// </summary>
        public static string NormalizeKey(string inputKey)
        {
            if (string.IsNullOrWhiteSpace(inputKey))
            {
                return string.Empty;
            }

            string normalizedKey = inputKey.ToLowerInvariant().Normalize(NormalizationForm.FormD);

            StringBuilder stringBuilder = new StringBuilder();

            foreach (char normalizedChar in normalizedKey)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(normalizedChar) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(normalizedChar);
                }
            }

            string cleanedKey = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            while (cleanedKey.Length > 0 &&
                   char.IsPunctuation(cleanedKey[cleanedKey.Length - 1]) &&
                   !char.IsDigit(cleanedKey[cleanedKey.Length - 1]))
            {
                cleanedKey = cleanedKey.Substring(0, cleanedKey.Length - 1);
            }

            int i = 0;

            while (i < cleanedKey.Length && char.IsDigit(cleanedKey[i]))
            {
                i++;
            }

            if (i > 0 && i < cleanedKey.Length)
            {
                cleanedKey = cleanedKey.Substring(0, i);
            }

            return cleanedKey;
        }
    }
}
