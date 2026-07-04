/// <file>SmartSearch.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LifeProManager
{
    /// <summary>
    /// Multilingual natural‑language search engine.
    /// All language‑specific data is stored in LangDict.cs, which is the only file
    /// that needs to be edited to add support for a new language.
    /// SmartSearch itself is fully language‑agnostic and relies entirely on the
    /// dictionaries for linguistic variations.
    /// </summary>
    public class SmartSearch
    {
        // Defines the main search modes: lexical, temporal, and hybrid.
        private enum QueryType
        {
            Lexical = 0,
            Temporal = 1,
            Hybrid = 2
        }

        // Static separators used for tokenization
        private static readonly char[] TokenSeparators = new[]
        {
            ' ', ',', ';', '.', ':', '!', '?', '/', '\\', '_', '(', ')', '[', ']', '{', '}'
        };

        // Reuses the global DB connection created in Program.cs
        public DBConnection dbConn => Program.DbConn;

        public SmartSearch()
        {

        }

        /// <summary>
        /// Builds the SQL Where clause used to retrieve candidate tasks.
        /// 
        /// This method unifies all search signals into a single SQL filter:
        /// - Textual fuzzy search (lexical tokens only)
        /// - Explicit date intervals (start/end)
        /// - Month‑based filtering
        /// - Semantic priority categories
        ///
        /// All generated conditions are combined with And.
        /// </summary>
        private string BuildSqlWhere(HashSet<string> expandedLexicalTokens,
            DateTime? startDate, DateTime? endDate, DateTime? detectedMonth,
            string priorityCategory, bool onlyTemporalTokens,
            out List<SqliteParameter> lstSqliteParameters)
        {
            lstSqliteParameters = new List<SqliteParameter>();
            HashSet<string> sqlConditions = new HashSet<string>();

            // ---------------------------------------------------------------------
            // PURE TEMPORAL QUERY : NO LEXICAL FILTERING
            // ---------------------------------------------------------------------
            if (onlyTemporalTokens)
            {
                return "1=1";
            }

            // ---------------------------------------------------------------------
            // EXPLICIT DATE RANGE (FROM NLP)
            // ---------------------------------------------------------------------
            if (startDate.HasValue && endDate.HasValue)
            {
                string paramStart = "@startDate";
                string paramEnd = "@endDate";

                sqlConditions.Add($"(date(deadline) >= date({paramStart}) AND date(deadline) <= date({paramEnd}))");

                lstSqliteParameters.Add(new SqliteParameter(paramStart, startDate.Value.ToString("yyyy-MM-dd")));
                lstSqliteParameters.Add(new SqliteParameter(paramEnd, endDate.Value.ToString("yyyy-MM-dd")));
            }

            // ---------------------------------------------------------------------
            // MONTH FILTER
            // ---------------------------------------------------------------------
            if (detectedMonth.HasValue)
            {
                DateTime monthStart = detectedMonth.Value;
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

                string paramMonthStart = "@monthStart";
                string paramMonthEnd = "@monthEnd";

                sqlConditions.Add($"(deadline >= {paramMonthStart} AND deadline <= {paramMonthEnd})");

                lstSqliteParameters.Add(new SqliteParameter(paramMonthStart, monthStart.ToString("yyyy-MM-dd")));
                lstSqliteParameters.Add(new SqliteParameter(paramMonthEnd, monthEnd.ToString("yyyy-MM-dd")));
            }

            // ---------------------------------------------------------------------
            // PRIORITY FILTER
            // ---------------------------------------------------------------------
            if (!string.IsNullOrEmpty(priorityCategory))
            {
                if (priorityCategory == "important")
                {
                    sqlConditions.Add("Priorities_id IN (1,3)");
                }
                else if (priorityCategory == "anniversary")
                {
                    sqlConditions.Add("Priorities_id = 4");
                }
            }

            // ---------------------------------------------------------------------
            // FINAL ASSEMBLY
            // ---------------------------------------------------------------------
            if (sqlConditions.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(" AND ", sqlConditions);
        }

        /// <summary>
        /// Computes the Levenshtein distance between two strings.
        /// The Levenshtein distance represents the minimum number of
        /// single‑character edits (insertions, deletions, substitutions)
        /// required to transform one string into the other.
        /// </summary>
        /// <remarks>
        /// This function builds a dynamic programming table that compares
        /// both strings character by character.  
        /// Each cell stores the minimum number of edits required to transform
        /// the prefix of the source string into the prefix of the target string.  
        /// The algorithm fills the entire table, so its time and memory usage
        /// grow proportionally with the lengths of the two input strings.
        /// </remarks>
        /// <param name="source">
        /// The original string from which the transformation begins.
        /// </param>
        /// <param name="target">
        /// The string to which the source string is compared and transformed.
        /// </param>
        /// <returns>The Levenshtein edit distance between the two strings.</returns>
        private static int CalculateLevenshteinDistance(string source, string target)
        {
            // Returns a very large value if either string is null
            if (source == null || target == null)
            {
                return int.MaxValue;
            }

            int sourceLength = source.Length;
            int targetLength = target.Length;

            // If the source is empty, the distance is the number of insertions needed
            if (sourceLength == 0)
            {
                return targetLength;
            }

            // If the target is empty, the distance is the number of deletions needed
            if (targetLength == 0)
            {
                return sourceLength;
            }

            // Creates a 2D matrix where each cell represents a subproblem
            // of transforming substrings of source and target
            int[,] distanceMatrix = new int[sourceLength + 1, targetLength + 1];

            // Initializes the first column by transforming source into an empty string
            for (int indexSource = 0; indexSource <= sourceLength; indexSource++)
            {
                // Cost of deleting characters from source
                distanceMatrix[indexSource, 0] = indexSource;
            }

            // Initializes the first row by transforming an empty string into target
            for (int indexTarget = 0; indexTarget <= targetLength; indexTarget++)
            {
                // Cost of inserting characters into source
                distanceMatrix[0, indexTarget] = indexTarget;
            }

            // Iterates through each character of the source string
            for (int indexSource = 1; indexSource <= sourceLength; indexSource++)
            {
                // Iterates through each character of the target string
                for (int indexTarget = 1; indexTarget <= targetLength; indexTarget++)
                {
                    // Determines whether the current characters match and sets the substitution cost accordingly
                    int substitutionCost = (source[indexSource - 1] == target[indexTarget - 1]) ? 0 : 1;

                    // Computes the cost of deleting a character from the source string
                    int deletionCost = distanceMatrix[indexSource - 1, indexTarget] + 1;

                    // Computes the cost of inserting a character into the source string
                    int insertionCost = distanceMatrix[indexSource, indexTarget - 1] + 1;

                    // Computes the cost of substituting one character for another in the source string
                    int substitutionTotalCost = distanceMatrix[indexSource - 1, indexTarget - 1] + substitutionCost;

                    // Selects the minimum cost among deletion, insertion, and substitution
                    int minimumCost = Math.Min(Math.Min(deletionCost, insertionCost), substitutionTotalCost);

                    // Stores the computed minimum cost in the matrix
                    distanceMatrix[indexSource, indexTarget] = minimumCost;
                }
            }

            // Returns the final computed distance (bottom‑right cell of the matrix)
            return distanceMatrix[sourceLength, targetLength];
        }

        /// <summary>
        /// Detects a semantic priority category from the normalized tokens.
        /// Each token is checked against LangDict.PriorityKeywordDict,
        /// which maps keywords (e.g., "important", "urgent") to priority categories.
        /// Returns the first matching category, or null if none is found.
        /// </summary>
        private string? DetectPriority(HashSet<string> normalizedTokens)
        {
            if (normalizedTokens == null || normalizedTokens.Count == 0)
            {
                return null;
            }

            foreach (string token in normalizedTokens)
            {
                if (LangDict.PriorityKeywordDict.TryGetValue(token, out string? priorityCategory))
                {
                    return priorityCategory;
                }
            }

            return null; // No priority detected
        }

        /// <summary>
        /// Determines whether the query is lexical, temporal, or hybrid.
        /// A token is considered temporal if it matches any known temporal
        /// keyword, unit, weekday, month, direction, or numeric date component.
        /// A query is:
        ///   - Temporal: only temporal tokens
        ///   - Lexical: only non‑temporal tokens
        ///   - Hybrid: a mix of both
        /// </summary>
        private QueryType DetermineQueryType(HashSet<string> tokens)
        {
            if (tokens == null || tokens.Count == 0)
            {
                return QueryType.Lexical;
            }

            bool containsTemporalToken = false;
            bool containsLexicalToken = false;

            foreach (string rawToken in tokens)
            {
                if (string.IsNullOrWhiteSpace(rawToken))
                {
                    continue;
                }

                string normalizedToken = LangDict.NormalizeKey(rawToken);

                bool isTemporalToken = normalizedToken.All(char.IsDigit) ||             // Pure numbers(years, days, offsets)
                    LangDict.TemporalDirectionDict.ContainsKey(normalizedToken) ||      // Temporal directions (next, last, previous)
                    LangDict.TemporalPrepositionSet.Contains(normalizedToken) ||        // Temporal prepositions (in, within, en, dans…)
                    LangDict.RelativeDayOffsetDict.ContainsKey(normalizedToken) ||      // Multi‑word collapsed offsets
                                                                                        //  (dayaftertomorrow, apresdemain…)
                    
                    
                    LangDict.WeekdayDict.ContainsKey(normalizedToken) ||                // Weekdays (monday, mardi…)                 
                    LangDict.MonthNumberDict.ContainsKey(normalizedToken) ||            // Months (march, mars, marzo…)
                    LangDict.DayKeywordSet.Contains(normalizedToken) ||                 // Units (day, week, month, year)
                    LangDict.WeekKeywordSet.Contains(normalizedToken) ||
                    LangDict.MonthKeywordSet.Contains(normalizedToken) ||
                    LangDict.YearKeywordSet.Contains(normalizedToken);

                if (isTemporalToken)
                {
                    containsTemporalToken = true;
                }
                else
                {
                    containsLexicalToken = true;
                }
            }

            if (containsTemporalToken && containsLexicalToken)
            {
                return QueryType.Hybrid;
            }

            if (containsTemporalToken)
            {
                return QueryType.Temporal;
            }

            return QueryType.Lexical;
        }

        /// <summary>
        /// Generates typo‑tolerant variant tokens for each token using Levenshtein distance.
        /// Temporal keywords are excluded from expansion to avoid incorrect matches.
        /// </summary>
        /// <param name="TokensSet">The set of original tokens extracted from the query.</param>
        /// <returns>A new set including typo‑tolerant variants.</returns>
        private HashSet<string> ExpandTokensLevenshtein(HashSet<string> TokensSet)
        {
            HashSet<string> ExpandedTokensSet = new HashSet<string>();

            if (TokensSet == null || TokensSet.Count == 0)
            {
                return ExpandedTokensSet;
            }

            foreach (string originalToken in TokensSet)
            {
                // Normalizes the token (removes accents, punctuation, etc.)
                string normalizedToken = LangDict.NormalizeKey(originalToken);

                // Includes the normalized token
                ExpandedTokensSet.Add(normalizedToken);

                // Skips Levenshtein expansion for temporal keywords
                if (LangDict.MonthNumberDict.ContainsKey(normalizedToken) ||       // months
                    LangDict.WeekdayDict.ContainsKey(normalizedToken) ||           // weekdays
                    LangDict.TemporalUnitDict.ContainsKey(normalizedToken) ||      // day, week, month, year
                    LangDict.TemporalDirectionDict.ContainsKey(normalizedToken) || // next, last, previous
                    LangDict.TemporalPrepositionSet.Contains(normalizedToken))     // in, within...
                {
                    continue;
                }

                // Skips expansion for long tokens (too many variants)
                if (normalizedToken.Length > 8)
                {
                    continue;
                }

                // Generates variant tokens from the normalized token
                List<string> lstVariantTokens = GenerateLevenshteinVariants(normalizedToken);

                foreach (string variant in lstVariantTokens)
                {
                    // Safety limit: prevents SQLite "too many variables" crash
                    if (ExpandedTokensSet.Count >= 200)
                    {
                        break;
                    }

                    // Adaptive Levenshtein radius:
                    // - short tokens (<= 6 chars) allow distance 2
                    // - longer tokens allow distance 1 to avoid pollution
                    int maxDistance = (normalizedToken.Length <= 6) ? 2 : 1;

                    int distance = CalculateLevenshteinDistance(normalizedToken, variant);

                    if (distance <= maxDistance && !ExpandedTokensSet.Contains(variant))
                    {
                        ExpandedTokensSet.Add(variant);
                    }

                }
            }

            return ExpandedTokensSet;
        }

        /// <summary>
        /// Extracts a 4‑digit year from tokens starting at the given index.
        /// Returns fallbackYear if no valid year is found.
        /// </summary>
        private int ExtractYear(List<string> normalizedTokens, int index, int fallbackYear)
        {
            // Checks if the token at the specified index is a 4‑digit number that can be parsed as a year.
            if (index < normalizedTokens.Count && normalizedTokens[index].Length == 4 &&
                int.TryParse(normalizedTokens[index], out int parsedYear))
            {
                return parsedYear;
            }

            return fallbackYear;
        }

        /// <summary>
        /// Generates typo‑tolerant variantTokens for a given token.
        /// This method produces simple character‑level mutations (insert, delete, replace)
        /// which are later filtered by Levenshtein distance ≤ 2.
        /// </summary>
        /// <param name="token">Single token for which typo‑tolerant variants are generated.</param>
        /// <returns>List of generated character‑level variants.</returns>

        private List<string> GenerateLevenshteinVariants(string token)
        {
            List<string> variantTokens = new List<string>();

            if (string.IsNullOrWhiteSpace(token))
            {
                return variantTokens;
            }

            string alphabet = "abcdefghijklmnopqrstuvwxyz";

            // Deletions (removes one character)
            for (int charToDelete = 0; charToDelete < token.Length; charToDelete++)
            {
                variantTokens.Add(token.Remove(charToDelete, 1));
            }

            // Insertions (inserts a character at any position)
            for (int charToInsert = 0; charToInsert <= token.Length; charToInsert++)
            {
                foreach (char letter in alphabet)
                {
                    variantTokens.Add(token.Insert(charToInsert, letter.ToString()));
                }
            }

            // Substitutions (replaces one character)
            for (int charToSubstitue = 0; charToSubstitue < token.Length; charToSubstitue++)
            {
                foreach (char letter in alphabet)
                {
                    if (token[charToSubstitue] != letter)
                    {
                        string mutated = token.Substring(0, charToSubstitue) + letter + token.Substring(charToSubstitue + 1);
                        variantTokens.Add(mutated);
                    }
                }
            }

            return variantTokens;
        }

        /// <summary>
        /// Cleans and normalizes the raw user query:
        /// - trims and collapses whitespace
        /// - removes punctuation and separators
        /// - protects logical operators (AND / OR)
        /// - converts to lowercase
        /// - removes accents and diacritics
        /// </summary>
        /// <param name="rawQuery">Raw user query</param>
        /// <returns>Lowercased, accent‑stripped version of the query.</returns>
        private string NormalizeText(string rawQuery)
        {
            if (string.IsNullOrWhiteSpace(rawQuery))
            {
                return string.Empty;
            }

            // Basic cleaning

            // Trims leading/trailing spaces
            rawQuery = rawQuery.Trim();

            // Collapses multiple spaces
            rawQuery = Regex.Replace(rawQuery, @"\s+", " ");

            // Normalizes separators
            rawQuery = rawQuery.Replace(",", " ").Replace(";", " ").Replace("/", " ");

            // Removes punctuation that has no semantic meaning
            rawQuery = Regex.Replace(rawQuery, @"[!?:()\[\]{}""'’]", "");

            // Protects logical operators by isolating them
            rawQuery = Regex.Replace(rawQuery, @"\bAND\b", " AND ", RegexOptions.IgnoreCase);
            rawQuery = Regex.Replace(rawQuery, @"\bOR\b", " OR ", RegexOptions.IgnoreCase);

            // Collapses spaces again after replacements
            rawQuery = Regex.Replace(rawQuery, @"\s+", " ");

            // Normalization

            // Lowercase
            rawQuery = rawQuery.ToLowerInvariant();

            // Decomposes accents (FormD)
            rawQuery = rawQuery.Normalize(NormalizationForm.FormD);

            // Removes diacritics
            var stringBuilder = new StringBuilder();
            
            foreach (char rawQueryChar in rawQuery)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(rawQueryChar) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(rawQueryChar);
                }
            }

            // Recomposes (FormC)
            rawQuery = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            return rawQuery.Trim();
        }

        /// <summary>
        /// Normalizes a single lexical token.
        /// Removes accents, strips punctuation, lowercases the token,
        /// and keeps only alphanumeric characters. 
        /// </summary>
        /// <param name="token">
        /// Raw token extracted from the user query.
        /// </param>
        /// <returns>
        /// A clean alphanumeric token (lowercase, accent‑free), 
        /// suitable for temporal and lexical matching.
        /// Returns an empty string if the input token contains no usable characters.
        /// </returns>
        private static string NormalizeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return string.Empty;
            }

            // Removes accents
            string normalizedToken = token.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char charToken in normalizedToken)
            {
                // Returns the Unicode category of the character
                if (CharUnicodeInfo.GetUnicodeCategory(charToken) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(charToken);
                }
            }

            string cleanedToken = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // Lowercase + remove punctuation
            cleanedToken = Regex.Replace(cleanedToken.ToLowerInvariant(), @"[^a-z0-9]", "");

            return cleanedToken;
        }

        /// <summary>
        /// Creates a dummy task indicating that the search crashed.
        /// This ensures the UI always receives a valid, non-empty result list.
        /// </summary>
        /// <returns>A fallback list containing a single crash‑indicator task.</returns>
        private List<Tasks> NotifyCrashResult()
        {
            Tasks crashTask = new Tasks
            {
                Id = -2,
                Title = LocalizationManager.GetString("SearchCrashedNoResults"),
                Description = string.Empty,
                Deadline = DateTime.Today.ToString("yyyy-MM-dd")
            };

            return new List<Tasks> { crashTask };
        }

        /// <summary>
        /// Resolves expressions like "next Monday" or "last Friday" into a concrete date.
        /// The method takes a weekday index and a direction (+1 for next, -1 for last),
        /// and computes the closest matching weekday in the appropriate direction.
        /// </summary>
        /// <param name="now">Reference date used as the starting point.</param>
        /// <param name="weekdayIndex">Numeric weekday index based on the WeekdayDict mapping.</param>
        /// <param name="directionSign">+1 for next, -1 for last.</param>
        /// <returns>The resolved target date.</returns>
        private DateTime ResolveDirectionalWeekday(DateTime now, DayOfWeek targetDay, int directionSign)
        {
            int currentDayNb = (int)now.DayOfWeek;
            int targetDayNb = (int)targetDay;

            if (directionSign > 0) // next
            {
                int delta = ((targetDayNb - currentDayNb + 7) % 7);
                
                if (delta == 0)
                {
                    delta = 7;
                }

                return now.Date.AddDays(delta);
            }

            else // last
            {
                int delta = ((currentDayNb - targetDayNb + 7) % 7);

                if (delta == 0)
                {
                    delta = 7;
                }

                return now.Date.AddDays(-delta);
            }
        }

        /// <summary>
        /// Computes the first and last day of the target month.
        /// </summary>
        private bool ResolveMonthRange(string rangeType, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            int offset;

            if (rangeType == "this")
            {
                offset = 0;
            }
            else if (rangeType == "next")
            {
                offset = +1;
            }
            else if (rangeType == "last")
            {
                offset = -1;
            }
            else
            {
                offset = 0;
            }

            int computedYear = now.Year;
            int computedMonth = now.Month + offset;

            // Normalizes month overflow
            while (computedMonth < 1)
            {
                computedMonth += 12;
                computedYear--;
            }

            while (computedMonth > 12)
            {
                computedMonth -= 12;
                computedYear++;
            }

            startDateTime = new DateTime(computedYear, computedMonth, 1);
            endDateTime = new DateTime(computedYear, computedMonth, DateTime.DaysInMonth(computedYear, computedMonth));

            return true;
        }

        /// <summary>
        /// Computes a relevance score for each task using:
        /// - exact matches in title and description
        /// - typo‑tolerant matches (Levenshtein distance)
        /// - match density (how many tokens appear)
        /// - token order (query order preserved)
        /// - deadline proximity (today, overdue, near, same month)
        /// </summary>
        private List<ScoredTask> ScoreCandidates(List<Tasks> candidateTasks,
                 HashSet<string> lexicalTokensOnly, HashSet<string> expandedLexicalTokens,
                 bool hasTemporal, string parsedPriority)
        {
            if (candidateTasks == null || candidateTasks.Count == 0)
            {
                return new List<ScoredTask>();
            }

            // Scoring weights for each scoring dimension
            var scoringWeight = new Dictionary<string, int>
            {
                ["ExactTitle"] = 40,
                ["ExactDescription"] = 25,
                ["ExpandedTitle"] = 12,
                ["ExpandedDescription"] = 8,
                ["Lev1"] = 6,
                ["Lev2"] = 3,
                ["Density"] = 4,
                ["TokenOrder"] = 10,
                ["FullCoverage"] = 8,
                ["DeadlineToday"] = 10,
                ["DeadlineOverdue"] = 12,
                ["DeadlineNear"] = 6,
                ["DeadlineSameMonth"] = 4,
                ["ExactTitlePrefix"] = 20,
                ["ExactDescriptionPrefix"] = 10,
                ["ExactTokenBoost"] = 15,
            };

            List<ScoredTask> scoredTasks = new List<ScoredTask>();

            foreach (Tasks task in candidateTasks)
            {
                // Normalizes title and description for consistent matching
                string taskTitle = NormalizeText(task.Title ?? "");
                string taskDescription = NormalizeText(task.Description ?? "");

                // Splits title and description into individual words: uses an array of separators
                // because .NET Framework requires a char[] for Split(), and RemoveEmptyEntries avoids blank tokens.
                var titleWords = taskTitle.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var descWords = taskDescription.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Remove spaces to catch merged words
                string titleConcat = taskTitle.Replace(" ", "");
                string descConcat = taskDescription.Replace(" ", "");

                // Insert spaces before camelCase transitions
                string titleCamel = Regex.Replace(taskTitle, "([a-z])([A-Z])", "$1 $2");
                string descCamel = Regex.Replace(taskDescription, "([a-z])([A-Z])", "$1 $2");

                // Split camelCase-expanded strings
                var camelWords = titleCamel.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Concat(descCamel.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                // Builds a unified list of all possible word forms for fuzzy matching
                List<string> allWords = titleWords
                    .Concat(descWords)
                    .Concat(camelWords)
                    .Concat(new[] { titleConcat, descConcat })
                    .Where(word => !string.IsNullOrWhiteSpace(word))
                    .Distinct(StringComparer.OrdinalIgnoreCase)   // Remove duplicates ignoring case
                    .ToList();

                // Tracks how many exact tokens appear in the task
                int exactMatchDensity = 0;

                // Tracks whether tokens appear in the same order as the query
                bool tokensRespectOrder = true;

                // Stores the last matched token index to check ordering
                int lastTokenIndex = -1;

                // Final score accumulator for this task
                int totalScore = 0;

                // ---------------------------------------------------------------------
                // TOKEN SCORING LOOP
                // ---------------------------------------------------------------------
                bool invalidForTemporalQuery = false;

                foreach (string expandedToken in expandedLexicalTokens)
                {
                    bool isExactToken = lexicalTokensOnly.Contains(expandedToken);
                    int tokenScore = 0;

                    // Finds token position in title or description
                    int posTitle = taskTitle.IndexOf(expandedToken, StringComparison.Ordinal);
                    int posDesc = taskDescription.IndexOf(expandedToken, StringComparison.Ordinal);

                    // Exacts or expanded match in title
                    if (posTitle >= 0)
                    {
                        tokenScore += scoringWeight[isExactToken ? "ExactTitle" : "ExpandedTitle"];

                        // Bonus for early title matches (prefix relevance)
                        tokenScore += (int)(scoringWeight["ExactTitlePrefix"] / (1 + posTitle));

                        // Extra boost for exact tokens
                        if (isExactToken)
                        {
                            tokenScore += scoringWeight["ExactTokenBoost"];
                        }
                    }
                    // Exact or expanded match in description
                    else if (posDesc >= 0)
                    {
                        tokenScore += scoringWeight[isExactToken ? "ExactDescription" : "ExpandedDescription"];

                        // Smaller bonus for early description matches
                        tokenScore += (int)(scoringWeight["ExactDescriptionPrefix"] / (1 + posDesc));
                    }

                    // Density: count how many exact tokens appear
                    if (isExactToken &&
                        (taskTitle.Contains(expandedToken) || taskDescription.Contains(expandedToken)))
                    {
                        exactMatchDensity++;
                    }

                    // Levenshtein fuzzy matching (only for original lexical tokens)
                    if (isExactToken)
                    {
                        // Plausibility filter to avoid fuzzy pollution
                        var plausibleWords = allWords
                            .Where(candidateWord =>
                            {
                                if (string.IsNullOrWhiteSpace(candidateWord))
                                {
                                    return false;
                                }

                                string normalizedCandidate = candidateWord.ToLowerInvariant();
                                string normalizedToken = expandedToken.ToLowerInvariant();

                                // Checks if both words start with the same letter
                                bool sameFirstLetter = normalizedCandidate[0] == normalizedToken[0];

                                // Counts how many characters they share
                                int sharedCharacterCount = normalizedCandidate.Intersect(normalizedToken).Count();

                                // Avoids matching words with very different lengths
                                bool similarLength = Math.Abs(normalizedCandidate.Length - normalizedToken.Length) <= 2;

                                // Accepts only if the candidate word is structurally plausible
                                return (sameFirstLetter || sharedCharacterCount >= 3) && similarLength;
                            })
                            .ToList();

                        // Computes best Levenshtein distance among plausible candidates
                        int bestDistance = plausibleWords
                            .Select(word => CalculateLevenshteinDistance(expandedToken, word))
                            .DefaultIfEmpty(int.MaxValue)
                            .Min();

                        // Strict mode when temporal tokens exist (e.g. "tomorrow office")
                        if (hasTemporal)
                        {
                            if (bestDistance > 1)
                            {
                                invalidForTemporalQuery = true;
                                break;
                            }

                            if (bestDistance == 1)
                            {
                                tokenScore += scoringWeight["Lev1"];
                            }
                            else if (bestDistance == 0)
                            {
                                tokenScore += scoringWeight["ExactTokenBoost"];
                            }
                        }

                        // Normal mode (no temporal tokens)
                        else
                        {
                            if (bestDistance <= 2)
                            {
                                string levelKey = bestDistance == 1 ? "Lev1" : "Lev2";
                                tokenScore += scoringWeight[levelKey];
                            }
                        }
                    }

                    // Token order detection
                    int indexInTitle = taskTitle.IndexOf(expandedToken);
                    int indexInDesc = taskDescription.IndexOf(expandedToken);
                    int tokenIndex = indexInTitle >= 0 ? indexInTitle : indexInDesc;

                    if (tokenIndex >= 0)
                    {
                        // If a token appears before a previous one, order is broken
                        if (lastTokenIndex > tokenIndex)
                        {
                            tokensRespectOrder = false;
                        }

                        lastTokenIndex = tokenIndex;
                    }

                    totalScore += tokenScore;
                }

                // If a token failed strict temporal matching, skips this task entirely
                if (invalidForTemporalQuery)
                {
                    continue;
                }

                // Density bonus: more exact tokens = more relevance
                totalScore += exactMatchDensity * scoringWeight["Density"];

                // Full coverage: all tokens appear somewhere
                if (expandedLexicalTokens.All(expandedToken =>
                    taskTitle.IndexOf(expandedToken, StringComparison.OrdinalIgnoreCase) >= 0
                    || taskDescription.IndexOf(expandedToken, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    totalScore += scoringWeight["FullCoverage"];
                }

                // Token order bonus
                if (tokensRespectOrder)
                {
                    totalScore += scoringWeight["TokenOrder"];
                }

                // Deadline scoring (temporal relevance)
                if (!string.IsNullOrWhiteSpace(task.Deadline) &&
                    DateTime.TryParse(task.Deadline, out DateTime deadline))
                {
                    DateTime today = DateTime.Today;
                    deadline = deadline.Date;

                    if (deadline == today)
                    {
                        totalScore += scoringWeight["DeadlineToday"];
                    }
                    if (deadline < today)
                    {
                        totalScore += scoringWeight["DeadlineOverdue"];
                    }
                    if (Math.Abs((deadline - today).TotalDays) <= 2)
                    {
                        totalScore += scoringWeight["DeadlineNear"];
                    }
                    if (deadline.Month == today.Month)
                    {
                        totalScore += scoringWeight["DeadlineSameMonth"];
                    }
                }

                // Penalizes tasks that have no exact lexical match at all
                if (exactMatchDensity == 0)
                {
                    totalScore -= 25;
                }

                // Priority semantic boost
                if (!string.IsNullOrEmpty(parsedPriority))
                {
                    string taskPriority = "";

                    // Map Priorities_id to semantic category
                    switch (task.Priorities_id)
                    {
                        case 1: // important
                        case 3: // important + repeatable
                            taskPriority = "important";
                            break;

                        case 4: // anniversary
                            taskPriority = "anniversary";
                            break;

                        default:
                            taskPriority = ""; // no semantic category
                            break;
                    }

                    if (taskPriority == parsedPriority)
                    {
                        // Strong semantic match
                        totalScore += 40;
                    }
                    else
                    {
                        // Weak semantic match (same family)
                        totalScore += 10;
                    }
                }


                // Adds final scored task only if not rejected
                if (totalScore > int.MinValue)
                {
                    scoredTasks.Add(new ScoredTask
                    {
                        Task = task,
                        Score = totalScore
                    });
                }

            }

            return scoredTasks;
        }

        /// <summary>
        /// Unified smart search combining lexical fuzzy search and temporal NLP.
        /// The query is classified as lexical, temporal, or hybrid.
        /// Each mode has its own filtering and sorting strategy.
        /// </summary>
        public List<Tasks> Search(string rawQuery)
        {
            try
            {
                string normalizedQuery = NormalizeText(rawQuery);

                // Tokenizes the query
                List<string> rawTokens = TokenizeQuery(normalizedQuery);

                // ---------------------------------------------------------------------
                // SPLIT LEXICAL TOKENS VS TEMPORAL TOKENS
                // ---------------------------------------------------------------------
                HashSet<string> lexicalTokensOnly = new HashSet<string>();
                HashSet<string> temporalTokensOnly = new HashSet<string>();

                foreach (string token in rawTokens)
                {
                    string normalizedToken = LangDict.NormalizeKey(token);

                    bool isTemporal =
                        normalizedToken.All(char.IsDigit) ||
                        LangDict.TemporalDirectionDict.ContainsKey(normalizedToken) ||
                        LangDict.TemporalPrepositionSet.Contains(normalizedToken) ||
                        LangDict.RelativeDayOffsetDict.ContainsKey(normalizedToken) ||
                        LangDict.WeekdayDict.ContainsKey(normalizedToken) ||
                        LangDict.MonthNumberDict.ContainsKey(normalizedToken) ||
                        LangDict.DayKeywordSet.Contains(normalizedToken) ||
                        LangDict.WeekKeywordSet.Contains(normalizedToken) ||
                        LangDict.MonthKeywordSet.Contains(normalizedToken) ||
                        LangDict.YearKeywordSet.Contains(normalizedToken);

                    if (isTemporal)
                    {
                        temporalTokensOnly.Add(normalizedToken);
                    }
                    else
                    {
                        lexicalTokensOnly.Add(normalizedToken);
                    }
                }

                // ---------------------------------------------------------------------
                // TEMPORAL NLP
                // ---------------------------------------------------------------------
                (DateTime? startDate, DateTime? endDate) = TryParseNaturalDates(rawTokens, rawQuery, DateTime.Today);

                // Determines query type using both sets
                QueryType queryType = DetermineQueryType(lexicalTokensOnly.Union(temporalTokensOnly).ToHashSet());

                // ---------------------------------------------------------------------
                // SHOW ALL COMMANDS
                // ---------------------------------------------------------------------
                if (lexicalTokensOnly.Count == 1 &&
                    LangDict.ShowAllKeywords.Contains(lexicalTokensOnly.First()))
                {
                    return dbConn.ReadTask("");
                }

                // ---------------------------------------------------------------------
                // FUZZY EXPANSION (LEXICAL TOKENS ONLY)
                // ---------------------------------------------------------------------
                HashSet<string> expandedLexicalTokens = ExpandTokensLevenshtein(lexicalTokensOnly);

                // Priority detection
                string? parsedPriority = DetectPriority(lexicalTokensOnly);

                // If the query contains only one lexical token and it is a priority keyword,
                // fuzzy expansion is disabled to avoid generating irrelevant variants.
                if (!string.IsNullOrEmpty(parsedPriority) && lexicalTokensOnly.Count == 1)
                {
                    expandedLexicalTokens.Clear();
                }

                // ---------------------------------------------------------------------
                // YEAR OVERRIDE
                // ---------------------------------------------------------------------
                foreach (string token in lexicalTokensOnly)
                {
                    if (token.Length == 4 && int.TryParse(token, out int parsedYear))
                    {
                        startDate = new DateTime(parsedYear, 1, 1);
                        endDate = new DateTime(parsedYear, 12, 31);
                        break;
                    }
                }

                // ---------------------------------------------------------------------
                // SQL FILTERING (LEXICAL TOKENS ONLY)
                // ---------------------------------------------------------------------
                string sqlWhere;
                List<SqliteParameter> sqlParams;

                sqlWhere = BuildSqlWhere(expandedLexicalTokens, startDate, endDate, null,
                    parsedPriority!, lexicalTokensOnly.Count == 0, out sqlParams
                );

                List<Tasks> candidateTasks = dbConn.SearchTasks(sqlWhere, sqlParams);

                // ---------------------------------------------------------------------
                // SCORING (LEXICAL ONLY)
                // ---------------------------------------------------------------------
                // A query is considered temporal as soon as it contains at least one temporal token,
                // even if TryParseNaturalDates() failed to build a concrete date range.
                bool hasTemporal = temporalTokensOnly.Count > 0;

                List<ScoredTask> scoredTasks = ScoreCandidates(candidateTasks, lexicalTokensOnly,
                    expandedLexicalTokens, hasTemporal, parsedPriority!);

                // ---------------------------------------------------------------------
                // KEYWORD-FIRST OVERRIDE
                // If lexical tokens matched something (score > 0), ignores temporal filters.
                // ---------------------------------------------------------------------
                bool hasLexicalMatch = scoredTasks.Any(scoredTask => scoredTask.Score > 0);

                if (hasLexicalMatch && lexicalTokensOnly.Count > 0)
                {
                    return scoredTasks
                        .Where(scoredTask => scoredTask.Score > 0)
                        .OrderByDescending(scoredTask => scoredTask.Score)
                        .Select(scoredTask => scoredTask.Task)
                        .ToList();
                }

                // ---------------------------------------------------------------------
                // TEMPORAL-ONLY MODE
                // ---------------------------------------------------------------------
                if (queryType == QueryType.Temporal && startDate.HasValue && endDate.HasValue)
                {
                    List<Tasks> filteredByDate = candidateTasks
                        .Where(task =>
                            !string.IsNullOrWhiteSpace(task.Deadline) &&
                            TryParseDeadlineUniversal(task.Deadline, out DateTime parsedDate) &&
                            parsedDate.Date >= startDate.Value.Date &&
                            parsedDate.Date <= endDate.Value.Date)
                        .OrderBy(task => task.Deadline)
                        .ToList();

                    return filteredByDate;
                }

                // ---------------------------------------------------------------------
                // LEXICAL-ONLY MODE
                // ---------------------------------------------------------------------
                if (queryType == QueryType.Lexical)
                {
                    lexicalTokensOnly = lexicalTokensOnly
                        .Where(token => token.Length >= 3 || token.All(char.IsDigit))
                        .ToHashSet();

                    return scoredTasks
                        .Where(scoredTask => scoredTask.Score > 0)
                        .OrderByDescending(scoredTask => scoredTask.Score)
                        .Select(scoredTask => scoredTask.Task)
                        .ToList();
                }

                // ---------------------------------------------------------------------
                // HYBRID MODE
                // ---------------------------------------------------------------------
                bool temporalWindowMissing = !startDate.HasValue || !endDate.HasValue;

                // Hybrid fallback (temporal failed)
                if (queryType == QueryType.Hybrid && temporalWindowMissing)
                {
                    return scoredTasks
                        .Where(scoredTask => scoredTask.Score > 0)
                        .OrderByDescending(scoredTask => scoredTask.Score)
                        .Select(scoredTask => scoredTask.Task)
                        .ToList();
                }

                // Hybrid with valid temporal window
                if (queryType == QueryType.Hybrid && startDate.HasValue && endDate.HasValue)
                {
                    // LINQ Syntax : 
                    // .Where              : condition that keeps what interests us
                    // .OrderByDescending  : sorts from the best to the least
                    // .ThenBy             : if equal, sorts by a second criterion
                    // .Select             : transforms each element
                    // .ToList             : materializes the final list
                    var hybridMatches = scoredTasks
                        .Where(scoredTask =>
                            !string.IsNullOrWhiteSpace(scoredTask.Task.Deadline) &&
                            TryParseDeadlineUniversal(scoredTask.Task.Deadline, out DateTime parsedDate) &&
                            parsedDate.Date >= startDate.Value.Date &&
                            parsedDate.Date <= endDate.Value.Date)
                        .OrderByDescending(scoredTask => scoredTask.Score)
                        .ThenBy(scoredTask => scoredTask.Task.Deadline)
                        .Select(scoredTask => scoredTask.Task)
                        .ToList();

                    if (hybridMatches.Count > 0)
                    {
                        return hybridMatches;
                    }
                }

                // ---------------------------------------------------------------------
                // GLOBAL FALLBACK
                // ---------------------------------------------------------------------
                var finalResults = scoredTasks
                     .Where(scoredTask => scoredTask.Score > 0)
                     .OrderByDescending(scoredTask => scoredTask.Score)
                     .Select(scoredTask => scoredTask.Task)
                     .ToList();

                // If lexical query but fuzzy scoring returned nothing,
                // retry with lexical-only broad match (no temporal filter).
                if (finalResults.Count == 0 && lexicalTokensOnly.Count > 0)
                {
                    return candidateTasks
                        .OrderBy(task => task.Deadline)
                        .ToList();
                }

                return finalResults;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SmartSearch Crash] {ex}");
                return NotifyCrashResult();
            }
        }

        /// <summary>
        /// Splits a pre-normalized query string into raw lexical tokens,
        /// then normalizes each token.
        /// </summary>
        /// <param name="query">
        /// The already normalized input string to tokenize.
        /// </param>
        /// <returns>
        /// A set of raw tokens extracted from the query
        /// </returns>
        private static List<string> TokenizeQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<string>();
            }

            return query
                .Split(TokenSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(token => NormalizeToken(token))
                .Where(token => !string.IsNullOrWhiteSpace(token))
                .ToList();
        }

        /// <summary>
        /// Applies a relative offset to a standard time unit ("day", "week", "month", "year").
        /// The method computes the resulting date or date range based on the quantity provided.
        /// </summary>
        /// <param name="offset">Positive or negative quantity representing the offset to apply.</param>
        /// <param name="normalizedUnit">Normalized time unit</param>
        /// <param name="now">Reference date used as the origin for the calculation.</param>
        /// <param name="startDateTime">Output: the computed start date of the resulting interval.</param>
        /// <param name="endDateTime">Output: the computed end date of the resulting interval.</param>
        /// <returns> True if the unit is supported and the offset was applied; otherwise false.</returns>

        private static bool TryApplyRelativeUnit(int offset, string normalizedUnit, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            // Days: simple offset on the current date
            if (normalizedUnit == "day")
            {
                startDateTime = now.Date.AddDays(offset);
                endDateTime = startDateTime;
                return true;
            }

            // Weeks: treated as complete intervals
            else if (normalizedUnit == "week")
            {
                startDateTime = now.Date.AddDays(offset * 7);
                endDateTime = startDateTime.Value.AddDays(6);
                return true;
            }

            // Months: uses AddMonths to keep calendar semantics
            else if (normalizedUnit == "month")
            {
                startDateTime = now.Date.AddMonths(offset);
                endDateTime = startDateTime;
                return true;
            }

            // Years: uses AddYears to keep calendar semantics
            else if (normalizedUnit == "year")
            {
                startDateTime = now.Date.AddYears(offset);
                endDateTime = startDateTime;
                return true;
            }

            // Unknown unit: nothing applied
            return false;
        }

        /// <summary>
        /// Tries to match absolute date keywords (today, tomorrow, yesterday)
        /// in supported languages.
        /// </summary>
        /// <param name="token">Single normalized token to evaluate.</param>
        /// <param name="now">Reference date used as the anchor.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date (same as start for this handler).</param>
        /// <returns>True if the token matches an absolute keyword.</returns>
        private static bool TryParseAbsoluteKeyword(string token, DateTime now, out DateTime? startDateTime,
            out DateTime? endDateTime)
        {
            startDateTime = endDateTime = null;

            if (LangDict.RelativeDayOffsetDict.TryGetValue(token, out int offset))
            {
                startDateTime = endDateTime = now.Date.AddDays(offset);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to parse an absolute date range from a list of tokens.
        /// Supports formulations such as: "from 3 to 7", "3 to 7", "3-7",
        /// "period from 3 to 7".
        /// </summary>
        /// <param name="tokensSet">Set of normalized tokens extracted from the query.</param>
        /// <param name="now">Reference date used as the anchor for range construction.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date if parsing succeeds.</param>
        /// <returns>True if a valid absolute numeric range is detected.</returns>
        public bool TryParseAbsoluteRangeTokens(HashSet<string> tokensSet, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            if (tokensSet == null || tokensSet.Count == 0)
            {
                return false;
            }

            // Normalizes tokens
            List<string> normalizedTokens = tokensSet
                .Select(token => LangDict.NormalizeKey(token))
                .ToList();

            // Ensures normalized range keyword sets are available
            if (LangDict.RangeOptionalPrefixSet == null || LangDict.RangeSeparatorSet == null)
            {
                return false;
            }

            // Detects separators ("au", "à", "to", etc.)
            int separatorIndex = -1;

            for (int i = 0; i < normalizedTokens.Count; i++)
            {
                if (LangDict.RangeSeparatorSet.Contains(normalizedTokens[i]))
                {
                    separatorIndex = i;
                    break;
                }
            }

            if (separatorIndex <= 0 || separatorIndex >= normalizedTokens.Count - 1)
            {
                return false;
            }

            // Left and right segments
            HashSet<string> leftTokens = tokensSet.Take(separatorIndex).ToHashSet();
            HashSet<string> rightTokens = tokensSet.Skip(separatorIndex + 1).ToHashSet();

            // Removes optional prefixes ("du", "from", "del", "période", "period", "periodo", etc.)
            leftTokens = leftTokens.Where(leftToken => 
            !LangDict.RangeOptionalPrefixSet.Contains(LangDict.NormalizeKey(leftToken))).ToHashSet();

            rightTokens = rightTokens.Where(rightToken => !LangDict.RangeOptionalPrefixSet.Contains(LangDict.NormalizeKey(rightToken)))
                .ToHashSet();

            // Parses left bound
            if (!TryParseDateTokens(leftTokens, now, out DateTime? leftStart, out DateTime? leftEnd))
            {
                return false;
            }

            // Parses right bound
            if (!TryParseDateTokens(rightTokens, now, out DateTime? rightStart, out DateTime? rightEnd))
            {
                return false;
            }

            // Chooses the start of each parsed date
            DateTime parsedStartDateTime = leftStart ?? leftEnd ?? default;
            DateTime parsedEndDateTime = rightStart ?? rightEnd ?? default;

            // Applies implicit month/year if needed
            if (parsedStartDateTime.Month == now.Month && leftTokens.Count == 1)
            {
                parsedStartDateTime = new DateTime(now.Year, now.Month, parsedStartDateTime.Day);
            }

            if (parsedEndDateTime.Month == now.Month && rightTokens.Count == 1)
            {
                parsedEndDateTime = new DateTime(now.Year, now.Month, parsedEndDateTime.Day);
            }

            // Rollover (e.g., 28th February to 3rd March, 30th December to 2nd January)
            if (parsedEndDateTime < parsedStartDateTime)
            {
                if (parsedEndDateTime.Month < parsedStartDateTime.Month)
                {
                    parsedEndDateTime = parsedEndDateTime.AddYears(1);
                }
            }

            startDateTime = parsedStartDateTime;
            endDateTime = parsedEndDateTime;
            return true;
        }

        /// <summary>
        /// Attempts to parse a directional "between X and Y" date range in a language‑agnostic way.
        /// The method relies on user‑editable keyword lists (lstBetweenKeywords, lstAndKeywords).
        /// Both X and Y are parsed using existing date handlers.
        /// Returns true only if both sides produce valid dates.
        /// </summary>
        /// <param name="input">Full normalized input string reconstructed from tokens.</param>
        /// <param name="now">Reference date used as the anchor for both parsed sides.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date if parsing succeeds.</param>
        /// <returns>True if a valid “between X and Y” date range is detected.</returns>
        public bool TryParseBetweenExpression(string input, DateTime now, out DateTime? startDate,
        out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            // Normalizes for keyword matching
            string normalizedInput = input.Trim().ToLowerInvariant();

            // Scans all "between" keywords (multi‑language)
            foreach (var betweenEntry in LangDict.lstBetweenKeywords)
            {
                string betweenKeyword = betweenEntry.value;
                string prefix = betweenKeyword + " ";

                // Input must start with the "between" keyword
                if (!normalizedInput.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Finds the matching "and" keyword for the same language
                string betweenEntryKey = betweenEntry.key;
                string? andKeyword = null;

                foreach (var andEntry in LangDict.lstAndKeywords)
                {
                    if (andEntry.key == betweenEntryKey)
                    {
                        andKeyword = andEntry.value;
                        break;
                    }
                }

                if (andKeyword == null)
                {
                    continue;
                }

                // Locates the "and" separator
                string separator = " " + andKeyword + " ";
                int separatorIndex = normalizedInput.IndexOf(separator, StringComparison.OrdinalIgnoreCase);

                // Must appear after the "between" prefix
                if (separatorIndex <= prefix.Length)
                {
                    continue;
                }

                // Extracts left and right segments
                string leftSegment = normalizedInput.Substring(prefix.Length, separatorIndex - prefix.Length).Trim();
                string rightSegment = normalizedInput.Substring(separatorIndex + separator.Length).Trim();

                // Parses both sides using the existing date segment parser
                if (TryParseDateSegment(leftSegment, now, out DateTime? leftStart, out DateTime? leftEnd) &&
                    TryParseDateSegment(rightSegment, now, out DateTime? rightStart, out DateTime? rightEnd))
                {
                    // Uses resolved dates (start or end depending on handler)
                    startDate = leftStart ?? leftEnd;
                    endDate = rightStart ?? rightEnd;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tokenizes a raw text segment and delegates date parsing to TryParseDateTokens.
        /// </summary>
        /// <param name="dateSegment">Raw substring representing a potential date expression.</param>
        /// <param name="now">Reference date used as the anchor for relative calculations.</param>
        /// <param name="startDate">Resolved start date if parsing succeeds.</param>
        /// <param name="endDate">Resolved end date if parsing succeeds.</param>
        /// <returns>True if the segment contains a valid date expression.</returns>
        private bool TryParseDateSegment(string dateSegment, DateTime now,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            if (string.IsNullOrWhiteSpace(dateSegment))
            {
                return false;
            }

            HashSet<string> segmentTokens = TokenizeQuery(dateSegment).ToHashSet();

            if (segmentTokens == null || segmentTokens.Count == 0)
            {
                return false;
            }

            return TryParseDateTokens(segmentTokens, now, out startDate, out endDate);
        }

        /// <summary>
        /// Main temporal parser.
        /// This method analyzes all tokens extracted from the query and tries to
        /// resolve a concrete date or date range. It supports many types of
        /// temporal expressions across multiple languages, including:
        ///
        /// • Relative expressions: "in 3 days", "in 2 weeks", "5 days ago"
        /// • Directional expressions: "next month", "last year", "mois suivant"
        /// • Absolute keywords: "today", "tomorrow", "yesterday",
        ///   "day after tomorrow", "avant-hier", etc.
        /// • Weekday expressions: "next Tuesday", "lundi prochain"
        /// • Month and year ranges: "this month", "last month",
        ///   "année passée", "next year"
        /// • Numeric dates: "14/03/2026", "2026-04-21", "14/03"
        /// • Ordinal dates: "3rd April", "1er mai"
        /// • Composite expressions: "in 2 months and 3 days",
        ///   "2 weeks and 3 days before"
        /// • Between ranges: "between 3 and 7"
        ///
        /// If a valid temporal expression is detected, the method returns a
        /// resolved start/end date range. Otherwise, it returns false.
        /// </summary>

        public bool TryParseDateTokens(HashSet<string> tokensSet, DateTime now,
        out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            if (tokensSet == null || tokensSet.Count == 0)
            {
                return false;
            }

            bool hasDigitToken = tokensSet.Any(token => token.Any(char.IsDigit));
            
            bool containsSemanticKeyword =
                tokensSet.Any(token => LangDict.WeekdayDict.ContainsKey(LangDict.NormalizeKey(token))) ||
                tokensSet.Any(token => LangDict.MonthNumberDict.ContainsKey(LangDict.NormalizeKey(token))) ||
                tokensSet.Any(token => LangDict.TemporalDirectionDict.ContainsKey(LangDict.NormalizeKey(token))) ||
                tokensSet.Any(token => LangDict.DayKeywordSet.Contains(LangDict.NormalizeKey(token))) ||
                tokensSet.Any(token => LangDict.MonthKeywordSet.Contains(LangDict.NormalizeKey(token)));

            if (!hasDigitToken && !containsSemanticKeyword)
            {
                
                return false;
            }

            // Freezes the order of the tokens
            List<string> tokensList = tokensSet.ToList();

            // Handlers who work on all tokens

            // Absolute ranges like "from 3 to 7", "3-7"
            if (TryParseAbsoluteRangeTokens(tokensSet, now, out startDateTime, out endDateTime))
            {
                return true;
            }

            // Between expressions like "between X and Y"
            string fullTokenInput = string.Join(" ", tokensList); 
            
            if (TryParseBetweenExpression(fullTokenInput, now, out startDateTime, out endDateTime))
            {
                return true;
            }

            // Multi-word month/year ranges 
            string normalizedFullTokenInput = LangDict.NormalizeKey(fullTokenInput);

            // Month-range expressions ("next month", "last month", "following month", etc.)
            if (LangDict.MonthRangeDict.TryGetValue(normalizedFullTokenInput, out string? monthRangeType))
            {
                int offset = monthRangeType == "next" ? +1 : 
                             monthRangeType == "last" ? -1 : 0;

                int year = now.Year;
                int month = now.Month + offset;

                while (month < 1) { month += 12; year--; }
                while (month > 12) { month -= 12; year++; }

                startDateTime = new DateTime(year, month, 1);
                endDateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                return true;
            }

            // Year-range expressions ("last year", "next year", "following year", etc.)
            if (LangDict.YearRangeDict.TryGetValue(normalizedFullTokenInput, out string? yearRangeType))
            {
                int offset = yearRangeType == "next" ? +1 :
                             yearRangeType == "last" ? -1 : 0;

                int targetYear = now.Year + offset;

                startDateTime = new DateTime(targetYear, 1, 1);
                endDateTime = new DateTime(targetYear, 12, 31);
                return true;
            }

            // ---------------------------------------------------------------------
            // Multi-word relative day expressions like "day after tomorrow"
            // Filter out non-semantic words using LangDict.IgnoredKeywordSet
            // before collapsing. This ensures correct matching against
            // RelativeDayOffsetDict.
            // ---------------------------------------------------------------------
            string filteredFullTokenInput = string.Join(" ",
                tokensList
                    .Select(token => LangDict.NormalizeKey(token))
                    .Where(token => !LangDict.IgnoredKeywordSet.Contains(token))
            );

            // Collapses only meaningful tokens
            string collapsedFullTokenInput = filteredFullTokenInput.Replace(" ", "");

            if (LangDict.RelativeDayOffsetDict.TryGetValue(collapsedFullTokenInput, out int relativeDayOffSetValue))
            {
                DateTime targetDateTime = now.Date.AddDays(relativeDayOffSetValue);
                startDateTime = targetDateTime;
                endDateTime = targetDateTime;
                return true;
            }

            for (int tokenIndex = 0; tokenIndex < tokensList.Count; tokenIndex++)
            {
                // Advanced relative composite expressions like "in 2 months and 3 days",
                if (TryParseRelativeCompositeExpression(tokensList, tokenIndex, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }

                // Simple relative expressions like "in 3 days"
                if (TryParseRelativeExpression(tokensList, tokenIndex, now, out startDateTime, out endDateTime))
                {
                    return true;
                }

                // Ago expressions like "5 days ago"
                if (TryParseRelativeAgoExpression(tokensList, tokenIndex, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }

                // Directional composite expressions like
                // "2 weeks and 3 days before" and "after 2 weeks and 3 days"
                if (TryParseRelativeDirectionalCompositeExpression(tokensList, tokenIndex, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }

                // Directional simple expressions:
                // "3 days before", "2 weeks after"
                if (TryParseRelativeDirectionalExpression(tokensList, tokenIndex, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }

                string currentToken = tokensList[tokenIndex];

                // Explicit numeric dates like 14/03/2026, 2026‑04‑21, 14/03
                if (TryParseNumericDateToken(currentToken, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }

                // Month expressions like this month, next month, last month
                if (TryParseMonthExpression(tokensSet, tokenIndex, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }

                // Ordinal dates like 3rd April
                if (TryParseOrdinalDate(tokensSet, tokenIndex, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }

                // Explicit years like 2026, next year
                if (TryParseYearExpression(tokensSet, tokenIndex, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }

                // Absolute keywords like today, tomorrow, yesterday
                if (TryParseAbsoluteKeyword(currentToken, now,
                        out startDateTime, out endDateTime))
                {
                    return true;
                }
            }

            // Weekday expressions like next Tuesday
            // Uses the dummy value "0", because it doesn't depend on the index.
            if (TryParseWeekdayExpression(tokensSet, 0, now, out startDateTime, out endDateTime))
            {
                return true;
            }

            // Explicit 4‑digit year fallback (e.g., "2026").
            // Order doesn't matter here.
            foreach (string rawToken in tokensSet)
            {
                string normalizedToken = LangDict.NormalizeKey(rawToken);

                if (normalizedToken.Length == 4 && int.TryParse(normalizedToken, out int explicitYear))
                {
                    startDateTime = new DateTime(explicitYear, 1, 1);
                    endDateTime = new DateTime(explicitYear, 12, 31);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Culture-proof deadline parser.
        /// First tries multi-culture DateTime.Parse, then falls back 
        /// to numeric extraction:
        ///   - YYYY MM DD
        ///   - DD MM YYYY
        ///   - MM DD YYYY
        /// </summary>
        private static bool TryParseDeadlineUniversal(string rawDeadline, out DateTime parsedDate)
        {
            parsedDate = default;

            if (string.IsNullOrWhiteSpace(rawDeadline))
            {
                return false;
            }

            string lowerCaseDeadline = rawDeadline.ToLower().Trim();

            // Multi-culture parsing (FR, ES, EN)
            CultureInfo[] lstCultures =
            {
                CultureInfo.InvariantCulture,
                new CultureInfo("fr-FR"),
                new CultureInfo("es-ES"),
                new CultureInfo("en-US"),
                new CultureInfo("en-GB")
            };

            foreach (CultureInfo culture in lstCultures)
            {
                if (DateTime.TryParse(lowerCaseDeadline, culture, DateTimeStyles.None, out parsedDate))
                {
                    return true;
                }
            }

            // Numeric fallback
            MatchCollection regexMatches = Regex.Matches(lowerCaseDeadline, @"\d+");
            List<string> lstDateDigits = new List<string>();

            foreach (Match regexMatch in regexMatches)
            {
                lstDateDigits.Add(regexMatch.Value);
            }

            if (lstDateDigits.Count < 3)
            {
                return false;
            }

            // Case 1 — YYYY MM DD
            if (lstDateDigits[0].Length == 4)
            {
                int parsedYear = int.Parse(lstDateDigits[0]);
                int parsedMonth = int.Parse(lstDateDigits[1]);
                int parsedDay = int.Parse(lstDateDigits[2]);

                parsedDate = new DateTime(parsedYear, parsedMonth, parsedDay);
                return true;
            }

            // Case 2 — DD MM YYYY or MM DD YYYY
            if (lstDateDigits[2].Length == 4)
            {
                int parsedYear = int.Parse(lstDateDigits[2]);
                int digitA = int.Parse(lstDateDigits[0]);
                int digitB = int.Parse(lstDateDigits[1]);

                int parsedDay = (digitA > 12) ? digitA : digitB;
                int parsedMonth = (digitA > 12) ? digitB : digitA;

                parsedDate = new DateTime(parsedYear, parsedMonth, parsedDay);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Matches month‑range expressions such as:
        /// "this month", "next month", "last month"
        /// in any supported language.
        /// </summary>
        /// <param name="TokensSet">Set of normalized tokens extracted from the query.</param>
        /// <param name="tokenIndex">Current token index used as the potential start of the pattern.</param>
        /// <param name="now">Reference date used as the anchor for relative calculations.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date (same as start for this handler).</param>
        /// <returns>True if a valid month‑range expression is detected.</returns>
        private bool TryParseMonthExpression(HashSet<string> TokensSet, int tokenIndex, DateTime now,
        out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            // Normalizes all tokens
            List<string> normalizedTokens = TokensSet
                .Select(token => LangDict.NormalizeKey(token))
                .ToList();

            // Builds a full normalized string
            string fullNormalizedString = string.Join(" ", normalizedTokens);

            // Tries full multi‑word match first
            if (LangDict.MonthRangeDict.TryGetValue(fullNormalizedString, out string? rangeType))
            {
                ResolveMonthRange(rangeType, now, out startDateTime, out endDateTime);
                return true;
            }

            // Tries all 2‑token combinations
            for (int i = 0; i < normalizedTokens.Count; i++)
            {
                for (int j = 0; j < normalizedTokens.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    string pairOfNormalizedTokens = normalizedTokens[i] + " " + normalizedTokens[j];

                    if (LangDict.MonthRangeDict.TryGetValue(pairOfNormalizedTokens, out rangeType))
                    {
                        ResolveMonthRange(rangeType, now, out startDateTime, out endDateTime);
                        return true;
                    }
                }
            }

            // Fallback: single token match (kept for compatibility)
            foreach (string token in normalizedTokens)
            {
                if (LangDict.MonthRangeDict.TryGetValue(token, out rangeType))
                {
                    ResolveMonthRange(rangeType, now, out startDateTime, out endDateTime);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Central natural‑language date parser.
        ///
        /// This method analyzes the raw user query and resolves any temporal
        /// expression it may contain. 
        /// It supports multi‑lingual patterns and applies them in a structured 
        /// priority order.
        ///
        /// Supported expression types include:
        /// • Multi‑word ranges: "this week", "next month", "last year",
        ///   "mois suivant", "année passée"
        /// • Relative day offsets: "day after tomorrow", "avant‑hier",
        ///   "pasado mañana"
        /// • Weekday + direction: "next Thursday", "lundi prochain"
        /// • Absolute keywords: "today", "tomorrow", "yesterday"
        /// • Standalone weekdays: "monday", "jeudi", "martes"
        /// • Composite relative expressions: "in 2 months and 3 days"
        /// • Directional relative expressions: "2 weeks before"
        /// • Simple relative expressions: "in 3 days"
        /// • Ordinal + month dates: "2 mars 2026"
        /// • Numeric dates: "14/03/2026", "march 5"
        /// • Explicit 4‑digit years: "2026"
        /// </summary>
        private (DateTime? start, DateTime? end) TryParseNaturalDates(List<string> orderedTokens,
            string rawQuery, DateTime today)
        {
            DateTime? startDate;
            DateTime? endDate;

            // Normalized forms used throughout the parser
            string normalizedQuery = LangDict.NormalizeKey(rawQuery);
            string collapsedQuery = normalizedQuery.Replace(" ", "");
            List<string> normalizedTokens = orderedTokens.Select(token => LangDict.NormalizeKey(token))
                .ToList();

            // ---------------------------------------------------------------------
            // Multi‑word temporal expressions (month/year/day)
            // ---------------------------------------------------------------------

            // Month ranges ("mois suivant", "following month", "mes proximo")
            if (LangDict.MonthRangeDict.TryGetValue(normalizedQuery, out string? monthRangeType))
            {
                int monthOffset = monthRangeType == "next" ? +1 :
                                  monthRangeType == "last" ? -1 : 0;

                int targetYear = today.Year;
                int targetMonth = today.Month + monthOffset;

                while (targetMonth < 1) 
                { 
                    targetMonth += 12; targetYear--; 
                }

                while (targetMonth > 12) 
                { 
                    targetMonth -= 12; targetYear++; 
                }

                return (new DateTime(targetYear, targetMonth, 1), new DateTime(targetYear, targetMonth,
                    DateTime.DaysInMonth(targetYear, targetMonth)));
            }

            // Year ranges ("année passée", "last year", "año pasado")
            if (LangDict.YearRangeDict.TryGetValue(normalizedQuery, out string? yearRangeType))
            {
                int yearOffset = yearRangeType == "next" ? +1 :
                                 yearRangeType == "last" ? -1 : 0;

                int resolvedYear = today.Year + yearOffset;

                return (new DateTime(resolvedYear, 1, 1), new DateTime(resolvedYear, 12, 31));
            }

            // Unified temporal ranges (week/month/year)
            if (TryParseTemporalRange(normalizedTokens, today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // Relative day offsets ("day after tomorrow", "apres demain", "pasado mañana")
            if (LangDict.RelativeDayOffsetDict.TryGetValue(collapsedQuery, out int dayOffset))
            {
                DateTime resolvedDay = today.AddDays(dayOffset);
                return (resolvedDay, resolvedDay);
            }

            // ---------------------------------------------------------------------
            // Weekday + direction ("next thursday", "lundi prochain")
            // ---------------------------------------------------------------------
            if (TryParseWeekdayDirectional(string.Join(" ", normalizedTokens), today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // ---------------------------------------------------------------------
            // Absolute keywords ("today", "tomorrow", "yesterday")
            // ---------------------------------------------------------------------
            if (TryParseAbsoluteKeyword(rawQuery, today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // ---------------------------------------------------------------------
            // Standalone weekday ("monday", "jeudi")
            // ---------------------------------------------------------------------
            if (TryParseWeekdayAbsolute(rawQuery, today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // ---------------------------------------------------------------------
            // Noun + weekday ("factura lunes", "meeting tuesday")
            // Detects patterns where a weekday appears after a non‑temporal word.
            // ---------------------------------------------------------------------
            for (int i = 0; i < normalizedTokens.Count - 1; i++)
            {
                string firstToken = normalizedTokens[i];
                string secondToken = normalizedTokens[i + 1];

                // Second token must be a weekday
                if (LangDict.WeekdayDict.TryGetValue(secondToken, out DayOfWeek weekday))
                {
                    // First token must not be temporal
                    if (!LangDict.TemporalDirectionDict.ContainsKey(firstToken) &&
                        !LangDict.TemporalPrepositionSet.Contains(firstToken) &&
                        !LangDict.MonthNumberDict.ContainsKey(firstToken) &&
                        !LangDict.WeekdayDict.ContainsKey(firstToken))
                    {
                        // Computes next occurrence of the weekday
                        int delta = ((int)weekday - (int)today.DayOfWeek + 7) % 7;
                        
                        if (delta == 0)
                        {
                            delta = 7; // always next week
                        }

                        DateTime targetDateTime = today.AddDays(delta);

                        startDate = targetDateTime;
                        endDate = targetDateTime;
                        return (startDate, endDate);
                    }
                }
            }

            // ---------------------------------------------------------------------
            // Composite relative expressions ("3 weeks and 4 days")
            // ---------------------------------------------------------------------
            List<string> rawTokens = TokenizeQuery(rawQuery);

            bool looksComposite = rawTokens.Count(token =>
            {
                string n = LangDict.NormalizeKey(token);
                return LangDict.DayKeywordSet.Contains(n)
                    || LangDict.WeekKeywordSet.Contains(n)
                    || LangDict.MonthKeywordSet.Contains(n)
                    || LangDict.YearKeywordSet.Contains(n);
            }) >= 2;

            if (looksComposite &&
                TryParseRelativeCompositeExpression(rawTokens, 0, today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // ---------------------------------------------------------------------
            // Directional relative expressions ("in 5 days", "2 weeks before")
            // ---------------------------------------------------------------------
            if (TryParseRelativeDirectionalExpression(normalizedTokens, 0, today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // ---------------------------------------------------------------------
            // Simple relative expressions ("in 3 days")
            // ---------------------------------------------------------------------
            if (TryParseRelativeExpression(normalizedTokens, 0, today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // ---------------------------------------------------------------------
            // Ordinal + month absolute dates ("2 mars 2026")
            // ---------------------------------------------------------------------
            if (TryParseOrdinalOfMonth(rawQuery, today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // ---------------------------------------------------------------------
            // Numeric dates ("12/03/2026", "march 5")
            // ---------------------------------------------------------------------
            if (TryParseDateTokens(normalizedTokens.ToHashSet(), today, out startDate, out endDate))
            {
                return (startDate, endDate);
            }

            // ---------------------------------------------------------------------
            // Explicit 4‑digit year ("2026")
            // ---------------------------------------------------------------------
            string? explicitYear = rawTokens
                .Select(token => LangDict.NormalizeKey(token))
                .FirstOrDefault(token => token.Length == 4 && int.TryParse(token, out _));

            if (explicitYear != null)
            {
                int parsedYear = int.Parse(explicitYear);
                return (new DateTime(parsedYear, 1, 1), new DateTime(parsedYear, 12, 31));
            }

            // ---------------------------------------------------------------------
            // Relative year expressions ("next year", "last year")
            // ---------------------------------------------------------------------
            bool containsYearKeyword = rawTokens.Any(token =>
                LangDict.YearKeywordSet.Contains(LangDict.NormalizeKey(token)));

            if (containsYearKeyword)
            {
                bool hasForwardKeyword = normalizedTokens.Any(token =>
                    LangDict.TemporalDirectionDict.TryGetValue(token, out int parsedDirection) && parsedDirection > 0);

                bool hasBackwardKeyword = normalizedTokens.Any(token =>
                    LangDict.TemporalDirectionDict.TryGetValue(token, out int parsedDirection) && parsedDirection < 0);

                int offset = hasForwardKeyword && !hasBackwardKeyword ? +1 :
                             hasBackwardKeyword && !hasForwardKeyword ? -1 : 0;

                int resolvedYear = today.Year + offset;

                return (new DateTime(resolvedYear, 1, 1),
                        new DateTime(resolvedYear, 12, 31));
            }

            // If no parser recognized a clear temporal expression,
            // returns (null, null) to avoid false positives.
            // This prevents accidental matches like "week" alone
            // or partial patterns that should not trigger temporal filtering.
            return (null, null);
        }

        /// <summary>
        /// Attempts to convert a number written in natural language
        /// into its integer value. Supports units, tens, and multipliers
        /// (hundred, thousand) as well as hyphenated or spaced forms.
        /// </summary> 
        /// <param name="inputWord">The word to process</param>
        /// <param name="parsedResult">The parsed number</param>
        /// <returns>True if the word represents a valid number in natural language.</returns>
        public static bool TryParseNumberWord(string inputWord, out int parsedResult)
        {
            parsedResult = 0;

            if (string.IsNullOrWhiteSpace(inputWord))
            {
                return false;
            }

            // Normalizes: remove hyphens, collapse spaces and lowercase
            string normalizedWord = inputWord.Replace("-", " ").Replace("‑", " ").Replace("  ", " ").Trim().ToLowerInvariant();
            
            // Normalizes French composite forms (70–99)
            normalizedWord = normalizedWord
                // 80 forms
                .Replace("quatre vingt", "quatre-vingt")
                .Replace("quatre‑vingt", "quatre-vingt")
                .Replace("quatrevingt", "quatre-vingt")
                // 70 forms
                .Replace("soixante dix", "soixante-dix")
                .Replace("soixante‑dix", "soixante-dix")
                .Replace("soixantedix", "soixante-dix");

            // Tokenizes after normalization
            string[] tokens = normalizedWord.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int currentGroupValue = 0; // Accumulates units/tens before a multiplier
            int finalValue = 0;        // Accumulates the full parsed number

            foreach (string token in tokens)
            {
                int unitValue;
                int tensValue;
                int multiplierValue;

                // Units (one, two, un, dos…)
                if (LangDict.NumberUnitDict.TryGetValue(token, out unitValue))
                {
                    currentGroupValue += unitValue;
                    continue;
                }

                // Tens (twenty, trente, treinta…)
                if (LangDict.NumberTenDict.TryGetValue(token, out tensValue))
                {
                    // French composite: 60 + (10–19) becomes 70–79
                    if (currentGroupValue == 60 && tensValue >= 10 && tensValue <= 19)
                    {
                        currentGroupValue = 60 + tensValue;
                        continue;
                    }

                    // French composite: 80 + (10–19) becomes 90–99
                    if (currentGroupValue == 80 && tensValue >= 10 && tensValue <= 19)
                    {
                        currentGroupValue = 80 + tensValue;
                        continue;
                    }

                    currentGroupValue += tensValue;
                    continue;
                }

                // Multipliers (hundred, cent, mille, thousand…)
                if (LangDict.NumberMultiplierDict.TryGetValue(token, out multiplierValue))
                {
                    // “cent” alone means 100, not 0 × 100
                    if (currentGroupValue == 0)
                    {
                        currentGroupValue = 1;
                    }

                    currentGroupValue *= multiplierValue;
                    finalValue += currentGroupValue;
                    currentGroupValue = 0;
                    continue;
                }

                // Unknown token: not a number word
                return false;
            }

            finalValue += currentGroupValue;
            parsedResult = finalValue;
            return true;
        }

        /// <summary>
        /// Attempts to parse a single token as an explicit numeric date.
        /// Supported formats:
        ///   - dd/MM/yyyy, dd‑MM‑yyyy
        ///   - yyyy/MM/dd, yyyy‑MM‑dd
        ///   - dd/MM, dd‑MM  (year defaults to the current year)
        ///
        /// This handler is lightweight and fast by using regex matching
        /// instead of culture‑dependent parsing.
        /// It is designed to be called inside the unified date‑token pipeline
        /// (TryParseDateTokens), so it only handles numeric formats.
        /// All textual month/day formats are handled by the other TryXXX handlers.
        /// </summary>
        /// <param name="token">Single token to evaluate as a numeric date.</param>
        /// <param name="now">Reference date used when the year is omitted.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date (same as start for this handler).</param>
        /// <returns>True if the token represents a valid numeric date.</returns>
        public bool TryParseNumericDateToken(string token, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            string trimmedToken = token.Trim();

            // Pattern: dd/MM/yyyy or dd-MM-yyyy
            Match matchDayMonthYear = Regex.Match(trimmedToken,
                @"^(?<day>\d{1,2})[\/\-](?<month>\d{1,2})[\/\-](?<year>\d{4})$");

            if (matchDayMonthYear.Success)
            {
                int parsedDay = int.Parse(matchDayMonthYear.Groups["day"].Value);
                int parsedMonth = int.Parse(matchDayMonthYear.Groups["month"].Value);
                int parsedYear = int.Parse(matchDayMonthYear.Groups["year"].Value);

                DateTime parsedDate = new DateTime(parsedYear, parsedMonth, parsedDay);
                startDateTime = parsedDate;
                endDateTime = parsedDate;
                return true;
            }

            // Pattern: yyyy/MM/dd or yyyy-MM-dd
            Match matchYearMonthDay = Regex.Match(trimmedToken,
                @"^(?<year>\d{4})[\/\-](?<month>\d{1,2})[\/\-](?<day>\d{1,2})$");

            if (matchYearMonthDay.Success)
            {
                int parsedYear = int.Parse(matchYearMonthDay.Groups["year"].Value);
                int parsedMonth = int.Parse(matchYearMonthDay.Groups["month"].Value);
                int parsedDay = int.Parse(matchYearMonthDay.Groups["day"].Value);

                DateTime parsedDate = new DateTime(parsedYear, parsedMonth, parsedDay);
                startDateTime = parsedDate;
                endDateTime = parsedDate;
                return true;
            }

            // Pattern: dd/MM or dd-MM : year defaults to now.Year
            Match matchDayMonth = Regex.Match(trimmedToken,
                @"^(?<day>\d{1,2})[\/\-](?<month>\d{1,2})$");

            if (matchDayMonth.Success)
            {
                int parsedDay = int.Parse(matchDayMonth.Groups["day"].Value);
                int parsedMonth = int.Parse(matchDayMonth.Groups["month"].Value);
                int parsedYear = now.Year;

                DateTime parsedDate = new DateTime(parsedYear, parsedMonth, parsedDay);
                startDateTime = parsedDate;
                endDateTime = parsedDate;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to extract a day number from an ordinal token in a fully language‑agnostic way.
        /// The method does not assume any alphabet or language: ordinal suffixes may come from
        /// any Unicode script (e.g., "st", "ème", "º", "日").
        ///
        /// Parsing strategy:
        /// - If the token is purely numeric ("1", "2", "15"), it is accepted directly.
        /// - Otherwise, the method checks whether the token ends with any known ordinal suffix.
        /// - If a suffix matches, the suffix is removed and the remaining part is parsed as a number.
        /// </summary>
        /// <param name="token">Raw token potentially containing an ordinal day.</param>
        /// <param name="dayNumber">Extracted day number if parsing succeeds.</param>
        /// <param name="hasOrdinalSuffix">True if the token contained a recognized ordinal suffix.</param>
        /// <returns>True if the token contains a valid ordinal day.</returns>
        private bool TryParseOrdinalDay(string token, out int dayNumber, out bool hasOrdinalSuffix)
        {

            dayNumber = 0;
            hasOrdinalSuffix = false;

            // Pure numeric day ("1", "2", "15")
            if (int.TryParse(token, out int numericDay))
            {
                dayNumber = numericDay;
                return true;
            }

            // Iterates through all known ordinal suffixes and checks whether
            // the token ends with one of them.
            // If a suffix matches, the suffix is removed and the remaining part
            // is parsed as the numeric day.
            // This works for any alphabet, since EndsWith() and Substring()
            // are Unicode‑safe.
            foreach (string suffix in LangDict.OrdinalSuffixSet)
            {
                if (token.EndsWith(suffix, StringComparison.Ordinal))
                {
                    string numericPart = token.Substring(0, token.Length - suffix.Length);

                    if (int.TryParse(numericPart, out int parsedDay))
                    {
                        dayNumber = parsedDay;
                        hasOrdinalSuffix = true;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Handles ordinal day expressions combined with day or month:
        /// - "7eme jour", "3rd day", "2do dia"
        /// - "7eme mars", "3rd april", "2do abril"
        /// - "7eme", "3rd", "2do" alone → day of current month
        /// Uses TryParseOrdinalDay() and monthDictionary.
        /// </summary>
        /// <param name="TokensSet">Set of normalized tokens extracted from the query.</param>
        /// <param name="tokenIndex">Current token index used as the potential start of the pattern.</param>
        /// <param name="now">Reference date used as the anchor for relative calculations.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date (same as start for this handler).</param>
        /// <returns>True if a valid ordinal‑day expression is detected.</returns>
        private bool TryParseOrdinalDate(HashSet<string> TokensSet, int tokenIndex, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            int dayNumber;
            bool hasOrdinalSuffix;

            // Accept ordinal day ("3rd", "7ème", "2do") or number word ("three", "trois", "tres") or digits ("3")
            if (!TryParseOrdinalDay(TokensSet.ElementAt(tokenIndex), out dayNumber, out hasOrdinalSuffix) &&
            !TryParseNumberWord(TokensSet.ElementAt(tokenIndex), out dayNumber) &&
            !int.TryParse(TokensSet.ElementAt(tokenIndex), out dayNumber))
            {
                return false;
            }

            // Case 1 — "7eme jour" / "3rd day" / "2do dia"
            if (tokenIndex + 1 < TokensSet.Count &&
                LangDict.DayKeywordSet.Contains(TokensSet.ElementAt(tokenIndex + 1)))
            {
                DateTime explicitDateChosen = new DateTime(now.Year, now.Month, dayNumber);
                startDateTime = explicitDateChosen;
                endDateTime = explicitDateChosen;
                return true;
            }

            // Case 2 — "7eme mars" / "3rd april" / "2do abril"
            if (tokenIndex + 1 < TokensSet.Count &&
                LangDict.MonthNumberDict.ContainsKey(TokensSet.ElementAt(tokenIndex + 1)))
            {
                int monthNumber = LangDict.MonthNumberDict[TokensSet.ElementAt(tokenIndex + 1)];

                // Explicit date chosen from ordinal day and month name
                DateTime explicitDateChosen = new DateTime(now.Year, monthNumber, dayNumber);

                startDateTime = explicitDateChosen;
                endDateTime = explicitDateChosen;
                return true;
            }

            // Case 3 — "7eme" / "3rd" / "2do" alone : interprets as day of current month
            if (hasOrdinalSuffix)
            {
                DateTime explicitDateChosen = new DateTime(now.Year, now.Month, dayNumber);
                startDateTime = explicitDateChosen;
                endDateTime = explicitDateChosen;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses absolute dates expressed in natural language formats:
        /// Examples:
        /// - "2nd of April 2026"
        /// - "2nd April 2026"
        /// - "April 2nd 2026"
        /// - "2 April 2026"
        /// Handles ordinal suffixes ("2nd", "3rd", "1st",...) and
        /// optional prepositions like "of".
        /// </summary>
        /// <param name="rawQuery">The raw user query.</param>
        /// <param name="today">Reference date (unused but kept for API consistency).</param>
        /// <param name="start">Resolved start date.</param>
        /// <param name="end">Resolved end date (same as start).</param>
        /// <returns>True if an absolute date was successfully parsed.</returns>
        private bool TryParseOrdinalOfMonth(string rawQuery, DateTime today,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            // Tokenizes raw query to preserve month names and prepositions
            List<string> rawTokens = TokenizeQuery(rawQuery);

            // If there are fewer than 2 tokens, it's impossible
            // to have a valid "<day> <month>" pattern.
            if (rawTokens.Count < 2)
            {
                return false;
            }

            // Normalized tokens for matching (accents removed, suffixes stripped)
            List<string> normalizedTokens = rawTokens
                .Select(token => LangDict.NormalizeKey(token))
                .ToList();

            for (int index = 0; index < normalizedTokens.Count; index++)
            {
                string normalizedToken = normalizedTokens[index];

                // Detects day number (1–31)
                if (!int.TryParse(normalizedToken, out int parsedDayNumber) || parsedDayNumber < 1 || parsedDayNumber > 31)
                {
                    continue;
                }

                // Pattern A: <day> <month> [year]
                if (index + 1 < normalizedTokens.Count &&
                    LangDict.MonthNumberDict.TryGetValue(normalizedTokens[index + 1], out int parsedMonthPatternA))
                {
                    // If the month is detected, we check for an optional year token right after it.
                    int parsedYear = ExtractYear(normalizedTokens, index + 2, today.Year);
                    startDateTime = new DateTime(parsedYear, parsedMonthPatternA, parsedDayNumber);
                    endDateTime = startDateTime;
                    return true;
                }

                // Pattern B: <day> <preposition> <month> [year]
                // Example: "2nd of April 2026", "2 de abril 2026", "le 2 mars 2026"
                if (index + 2 < normalizedTokens.Count)
                {
                    string candidatePreposition = normalizedTokens[index + 1];

                    bool isValidPreposition = LangDict.DatePrepositionSet.Contains(candidatePreposition);

                    if (isValidPreposition &&
                        LangDict.MonthNumberDict.TryGetValue(normalizedTokens[index + 2], out int parsedMonthPatternB))
                    {
                        int parsedYear = ExtractYear(normalizedTokens, index + 3, today.Year);

                        startDateTime = new DateTime(parsedYear, parsedMonthPatternB, parsedDayNumber);
                        endDateTime = startDateTime;

                        return true;
                    }
                }
            }

            // Pattern C: "<month> <day> [year]"
            // like "April 2nd 2026"
            for (int index = 0; index < normalizedTokens.Count; index++)
            {
                string normalizedToken = normalizedTokens[index];

                if (!LangDict.MonthNumberDict.TryGetValue(normalizedToken, out int parsedMonthNumber))
                {
                    continue;
                }

                if (index + 1 < normalizedTokens.Count &&
                    int.TryParse(normalizedTokens[index + 1], out int parsedDayNumber) &&
                    parsedDayNumber >= 1 && parsedDayNumber <= 31)
                {
                    int parsedYear = ExtractYear(normalizedTokens, index + 2, today.Year);
                    startDateTime = new DateTime(parsedYear, parsedMonthNumber, parsedDayNumber);
                    endDateTime = startDateTime;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Detects a standalone weekday token like "monday" or "wednesday".
        /// Matching is strict and token-based.
        /// Resolves to the next occurrence of that weekday, including today.
        /// </summary>
        /// <param name="descriptionText">Raw user input text.</param>
        /// <param name="now">Reference date.</param>
        /// <param name="startDate">Resolved start date.</param>
        /// <param name="endDate">Resolved end date.</param>
        /// <returns>True if a weekday is detected.</returns>
        private bool TryParseWeekdayAbsolute(string descriptionText, DateTime now,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            if (string.IsNullOrWhiteSpace(descriptionText))
            {
                return false;
            }

            List<string> normalizedTokens = TokenizeQuery(descriptionText)
                .Select(token => LangDict.NormalizeKey(token))
                .ToList();

            // Identifies the day of the week mentioned in the user’s sentence,
            // by comparing the normalized tokens with the key words of supported days.
            var weekdayEntry = LangDict.lstWeekdays.FirstOrDefault(weekDay => normalizedTokens.Contains(weekDay.key));

            if (weekdayEntry.key == null)
            {
                return false;
            }

            DayOfWeek targetDayOfWeek = weekdayEntry.value;

            int deltaValue = ((int)targetDayOfWeek - (int)now.DayOfWeek + 7) % 7;
            DateTime targetDateTime = now.Date.AddDays(deltaValue);

            startDate = targetDateTime;
            endDate = targetDateTime;
            return true;
        }

        /// <summary>
        /// Detects expressions combining a temporal direction (e.g. "next", "last")
        /// with a weekday, in any order ("next Thursday", "Thursday next").
        /// If detected, resolves the expression into a concrete date using
        /// ResolveDirectionalWeekday.
        /// </summary>
        /// <param name="descriptionText">Raw user input text.</param>
        /// <param name="now">Reference date used for resolution.</param>
        /// <param name="startDate">Resolved start date if parsing succeeds.</param>
        /// <param name="endDate">Resolved end date if parsing succeeds.</param>
        /// <returns>True if a directional weekday expression is detected.</returns>
        private bool TryParseWeekdayDirectional(string descriptionText, DateTime now,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            var tokens = TokenizeQuery(descriptionText)
                .Select(token => LangDict.NormalizeKey(token))
                .Where(token => !string.IsNullOrWhiteSpace(token))
                .ToList();

            if (tokens.Count < 2)
            {
                return false;
            }

            // Detects direction + weekday
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                string firstToken = tokens[i];
                string secondToken = tokens[i + 1];

                bool firstTokenIsDirection = LangDict.TemporalDirectionDict.TryGetValue(firstToken, out int directionSign);
                bool secondTokenIsWeekday = LangDict.WeekdayDict.TryGetValue(secondToken, out DayOfWeek weekday);

                if (firstTokenIsDirection && secondTokenIsWeekday)
                {
                    DateTime target = ResolveDirectionalWeekday(now, weekday, directionSign);
                    startDate = target;
                    endDate = target;
                    return true;
                }
            }

            // Detects weekday + direction
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                string firstToken = tokens[i];
                string secondToken = tokens[i + 1];

                bool firstTokenIsWeekday = LangDict.WeekdayDict.TryGetValue(firstToken, out DayOfWeek weekday);
                bool secondTokenIsDirection = LangDict.TemporalDirectionDict.TryGetValue(secondToken, out int directionSign);

                if (firstTokenIsWeekday && secondTokenIsDirection)
                {
                    DateTime target = ResolveDirectionalWeekday(now, weekday, directionSign);
                    startDate = target;
                    endDate = target;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parses relative expressions using "ago" semantics, such as:
        /// "3 days ago", "2 weeks ago", "il y a 5 jours", "hace 3 semanas",
        /// or any equivalent structure supported by LangDict.
        ///
        /// This handler detects:
        /// - a quantity (digit or number word)
        /// - a time unit ("day", "week", "month", "year", etc.)
        /// - a backward‑direction keyword ("ago", "il y a", "hace", ...)
        ///
        /// The method applies the negative offset to the reference date 'now'
        /// and returns a single resolved date (start = end).
        /// </summary>
        /// <param name="tokens">Normalized token list extracted from the query.</param>
        /// <param name="tokenIndex">Current token index used as the potential start of the pattern.</param>
        /// <param name="now">Reference date used as the anchor for relative calculations.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date (same as start for this handler).</param>
        /// <returns>True if a valid “ago” relative expression is detected.</returns>
        private bool TryParseRelativeAgoExpression(List<string> tokens, int tokenIndex, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            string[] tokensArray = tokens.ToArray();

            startDateTime = null;
            endDateTime = null;

            bool parseSuccessful = false;

            //
            // Pattern: "X unit ago"
            //
            if (tokenIndex + 2 < tokens.Count)
            {
                string quantityToken = tokensArray[tokenIndex];
                string normalizedUnit = LangDict.NormalizeKey(tokensArray[tokenIndex + 1]);
                string directionToken = LangDict.NormalizeKey(tokensArray[tokenIndex + 2]);

                bool quantityValid = TryParseNumberWord(quantityToken, out int quantityValue) ||
                    int.TryParse(quantityToken, out quantityValue);

                bool unitValid = LangDict.TemporalUnitDict.TryGetValue(normalizedUnit, out var canonicalUnit);

                bool directionValid = LangDict.TemporalDirectionDict.TryGetValue(directionToken, out int directionSign)
                    && directionSign == -1;

                // If one of these three is invalid, we return false immediately to avoid further processing.
                if (!quantityValid || !unitValid || !directionValid)
                {
                    return false;
                }

                int signedQuantity = quantityValue * -1;

                DateTime? computedStart;
                DateTime? computedEnd;

                bool offsetApplied = TryApplyRelativeUnit(
                    signedQuantity,
                    canonicalUnit!,   // guaranteed non-null because unitValid == true
                    now, out computedStart, out computedEnd);

                if (offsetApplied && computedStart.HasValue)
                {
                    startDateTime = computedStart;
                    endDateTime = computedStart;
                    parseSuccessful = true;
                }
            }

            //
            // Pattern: "il y a X unit"
            //
            if (!parseSuccessful && tokenIndex + 4 < tokens.Count)
            {
                string firstToken = LangDict.NormalizeKey(tokensArray[tokenIndex]);
                string secondToken = LangDict.NormalizeKey(tokensArray[tokenIndex + 1]);
                string thirdToken = LangDict.NormalizeKey(tokensArray[tokenIndex + 2]);

                string quantityToken = tokensArray[tokenIndex + 3];
                string unitToken = LangDict.NormalizeKey(tokensArray[tokenIndex + 4]);

                bool agoStructureValid = LangDict.AgoMultiTokenStructures.Any(structure =>
                    structure.Length == 3 &&
                    structure[0] == firstToken &&
                    structure[1] == secondToken &&
                    structure[2] == thirdToken);

                bool quantityValid =
                    TryParseNumberWord(quantityToken, out int quantityValue) ||
                    int.TryParse(quantityToken, out quantityValue);

                bool unitValid = LangDict.TemporalUnitDict.TryGetValue(unitToken, out var canonicalUnit);

                if (agoStructureValid && quantityValid && unitValid)
                {
                    int signedQuantity = quantityValue * -1;

                    DateTime? computedStart;
                    DateTime? computedEnd;

                    bool offsetApplied = TryApplyRelativeUnit(
                        signedQuantity,
                        canonicalUnit!,   // guaranteed non-null because unitValid == true
                        now, out computedStart, out computedEnd);

                    if (offsetApplied && computedStart.HasValue)
                    {
                        startDateTime = computedStart;
                        endDateTime = computedStart;
                        parseSuccessful = true;
                    }
                }
            }

            return parseSuccessful;
        }

        /// <summary>
        /// Parses composite directional relative expressions such as:
        /// "after 2 weeks and 3 days", "2 months and 5 days before",
        /// or any equivalent multi‑unit structure supported by LangDict.
        /// This handler detects and resolves expressions of the form:
        /// direction + [optional preposition] + quantity1 + unit1 + connector + quantity2 + unit2
        /// </summary>
        /// <param name="tokens">Normalized list of tokens extracted from the query.</param>
        /// <param name="tokenIndex">Index of the potential start of the composite expression.</param>
        /// <param name="now">Reference date used as the anchor for relative calculations.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date (same as start for this handler).</param>
        /// <returns>
        /// True if a valid composite directional relative expression is detected
        /// and successfully resolved; otherwise false.
        /// </returns>
        private bool TryParseRelativeDirectionalCompositeExpression(List<string> tokens,
            int tokenIndex, DateTime now, out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            // Needs: direction + qty1 + unit1 + connector + qty2 + unit2
            if (tokenIndex + 5 >= tokens.Count)
            {
                return false;
            }

            string directionToken = LangDict.NormalizeKey(tokens[tokenIndex]);

            // Uses unified temporal directions (+1 = after, -1 = before)
            if (!LangDict.TemporalDirectionDict.TryGetValue(directionToken, out int directionSign))
            {
                return false;
            }

            int nextIndex = tokenIndex + 1;

            // Optional preposition ("of", "de", "del", etc.)
            string possiblePreposition = LangDict.NormalizeKey(tokens[nextIndex]);

            if (LangDict.TemporalPrepositionSet.Contains(possiblePreposition))
            {
                nextIndex++;
            }

            // First quantity
            int quantity1;

            if (!TryParseNumberWord(tokens[nextIndex], out quantity1) && !int.TryParse(tokens[nextIndex], out quantity1))
            {
                return false;
            }

            nextIndex++;

            // First unit
            string unit1Token = LangDict.NormalizeKey(tokens[nextIndex]);

            if (!LangDict.TemporalUnitDict.TryGetValue(unit1Token, out string? canonicalUnit1))
            {
                return false;
            }

            nextIndex++;

            // Connector ("and", "et", "y")
            string connectorToken = LangDict.NormalizeKey(tokens[nextIndex]);

            if (!LangDict.AndKeywordSet.Contains(connectorToken))
            {
                return false;
            }

            nextIndex++;

            // Second quantity
            int quantity2;

            if (!TryParseNumberWord(tokens[nextIndex], out quantity2) &&
                !int.TryParse(tokens[nextIndex], out quantity2))
            {
                return false;
            }

            nextIndex++;

            // Second unit
            string unit2Token = LangDict.NormalizeKey(tokens[nextIndex]);

            if (!LangDict.TemporalUnitDict.TryGetValue(unit2Token, out string? canonicalUnit2))
            {
                return false;
            }

            int signedQuantity1 = quantity1 * directionSign;
            int signedQuantity2 = quantity2 * directionSign;

            DateTime? computedStart = now;
            DateTime? computedEnd = now;

            bool firstOffsetApplied = TryApplyRelativeUnit(
                signedQuantity1, canonicalUnit1, computedStart.Value,
                out computedStart, out computedEnd);

            if (!firstOffsetApplied || !computedStart.HasValue)
            {
                return false;
            }

            bool secondOffsetApplied = TryApplyRelativeUnit(signedQuantity2, canonicalUnit2, computedStart.Value,
                out computedStart, out computedEnd);

            if (!secondOffsetApplied || !computedStart.HasValue)
            {
                return false;
            }

            startDateTime = computedStart;
            endDateTime = computedStart;

            return true;
        }

        /// <summary>
        /// Parses directional relative expressions where the direction
        /// appears before the quantity, such as:
        /// "before 3 days", "after 2 weeks"
        /// </summary>
        /// <param name="tokens">Ordered list of normalized tokens.</param>
        /// <param name="tokenIndex">Index of the potential start of the pattern.</param>
        /// <param name="now">Reference date used as the anchor for relative calculations.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date if parsing succeeds.</param>
        /// <returns>True if a valid directional relative expression is detected.</returns>
        private bool TryParseRelativeDirectionalExpression(List<string> tokens, int tokenIndex,
         DateTime now, out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;


            // If the next token is a unit (day/week/month/year) but not a number,
            // this is not a directional-relative expression.
            if (tokenIndex + 1 < tokens.Count)
            {
                string nextToken = LangDict.NormalizeKey(tokens[tokenIndex + 1]);

                bool nextIsUnit =
                    LangDict.DayKeywordSet.Contains(nextToken) ||
                    LangDict.WeekKeywordSet.Contains(nextToken) ||
                    LangDict.MonthKeywordSet.Contains(nextToken) ||
                    LangDict.YearKeywordSet.Contains(nextToken);

                bool nextIsNumber = tokens[tokenIndex + 1].Any(char.IsDigit);

                if (nextIsUnit && !nextIsNumber)
                {
                    return false;
                }
            }

            // Local helpers
            bool TryResolveUnit(string token, out string? canonicalUnit)
            {
                string normalizedToken = LangDict.NormalizeKey(token);

                if (LangDict.DayKeywordSet.Contains(normalizedToken))
                {
                    canonicalUnit = "day";
                    return true;
                }

                if (LangDict.WeekKeywordSet.Contains(normalizedToken))
                {
                    canonicalUnit = "week";
                    return true;
                }

                if (LangDict.MonthKeywordSet.Contains(normalizedToken))
                {
                    canonicalUnit = "month";
                    return true;
                }

                if (LangDict.YearKeywordSet.Contains(normalizedToken))
                {
                    canonicalUnit = "year";
                    return true;
                }

                canonicalUnit = null;
                return false;
            }

            bool TryResolveQuantity(string token, out int quantity)
            {
                // Tries to interpret the token as a written number word like "three", "four".
                // If that fails, falls back to parsing it as a numeric string like "3", "4").
                return TryParseNumberWord(token, out quantity) || int.TryParse(token, out quantity);
            }

            // ---------------------------------------------------------------------
            // Pattern 1 : direction + quantity + unit
            // ---------------------------------------------------------------------
            if (tokenIndex + 2 < tokens.Count)
            {
                string directionToken = LangDict.NormalizeKey(tokens[tokenIndex]);
                string quantityToken = tokens[tokenIndex + 1];
                string unitToken = tokens[tokenIndex + 2];

                bool directionValid = LangDict.TemporalDirectionDict.TryGetValue(directionToken, out int directionSign);
                bool quantityValid = TryResolveQuantity(quantityToken, out int quantityValue);
                bool unitValid = TryResolveUnit(unitToken, out string? canonicalUnit);

                if (directionValid && quantityValid && unitValid)
                {
                    int signedQuantity = quantityValue * directionSign;

                    if (TryApplyRelativeUnit(signedQuantity, canonicalUnit!, now, out DateTime? computedStart, out _)
                        && computedStart.HasValue)
                    {
                        startDateTime = computedStart;
                        endDateTime = computedStart;
                        return true;
                    }
                }
            }

            // ---------------------------------------------------------------------
            // Pattern 2 : preposition + number + unit
            // ---------------------------------------------------------------------
            for (int i = 0; i < tokens.Count - 2; i++)
            {
                string prepositionToken = LangDict.NormalizeKey(tokens[i]);
                string quantityToken = tokens[i + 1];
                string unitToken = LangDict.NormalizeKey(tokens[i + 2]);

                if (LangDict.TemporalPrepositionSet.Contains(prepositionToken)
                    && int.TryParse(quantityToken, out int quantityValue)
                    && LangDict.TemporalUnitToDays.TryGetValue(unitToken, out int unitDays))
                {
                    DateTime computedDateTime = now.AddDays(quantityValue * unitDays);
                    startDateTime = computedDateTime;
                    endDateTime = computedDateTime;
                    return true;
                }
            }

            // ---------------------------------------------------------------------
            // Pattern 3 : direction + "de" + quantity + unit (Spanish)
            // ---------------------------------------------------------------------
            if (tokenIndex + 3 < tokens.Count)
            {
                string directionToken = LangDict.NormalizeKey(tokens[tokenIndex]);
                string middleToken = LangDict.NormalizeKey(tokens[tokenIndex + 1]);
                string quantityToken = tokens[tokenIndex + 2];
                string unitToken = tokens[tokenIndex + 3];

                bool directionValid = LangDict.TemporalDirectionDict.TryGetValue(directionToken, out int directionSign);
                bool middleValid = middleToken == "de";
                bool quantityValid = TryResolveQuantity(quantityToken, out int quantityValue);
                bool unitValid = TryResolveUnit(unitToken, out string? canonicalUnit);

                if (directionValid && middleValid && quantityValid && unitValid)
                {
                    int signedQuantity = quantityValue * directionSign;

                    if (TryApplyRelativeUnit(signedQuantity, canonicalUnit!, now, out DateTime? computedStart, out _)
                        && computedStart.HasValue)
                    {
                        startDateTime = computedStart;
                        endDateTime = computedStart;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Parses composite relative date expressions such as:
        /// "in 2 months and 3 days", "within 1 week and 2 days",
        /// or reversed directional forms like "2 weeks and 3 days before".
        ///
        /// This handler detects multi‑unit relative structures composed of:
        /// - a first quantity + time unit (e.g., "2 months")
        /// - a connector ("and", "et", "y")
        /// - a second quantity + time unit (e.g., "3 days")
        /// - an optional directional keyword ("before", "after", "avant", "después")
        ///
        /// The method applies both relative offsets in sequence, starting from 'now'.
        /// If a direction is present, both quantities are signed accordingly.
        /// The result is a single resolved date (start = end).
        /// </summary>
        /// <param name="tokens">Normalized token list extracted from the query.</param>
        /// <param name="tokenIndex">Current token index used as the potential start of the pattern.</param>
        /// <param name="now">Reference date used as the anchor for relative calculations.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date (same as start for this handler).</param>
        /// <returns>True if a valid composite relative expression is detected.</returns>
        private bool TryParseRelativeCompositeExpression(List<string> tokens, int tokenIndex,
            DateTime now, out DateTime? startDateTime, out DateTime? endDateTime)
        {
            // Initializes output interval
            startDateTime = null;
            endDateTime = null;

            // Tracks whether parsing succeeded
            bool parseSuccessful = false;

            // Pattern: "in/within X unit and Y unit"
            // Examples: "in 2 months and 3 days", "within 1 week and 2 days",

            // Ensures enough tokens for this pattern
            if (tokenIndex + 5 < tokens.Count)
            {
                // Normalizes the starting keyword ("in", "within", etc.)
                string startKeyword = LangDict.NormalizeKey(tokens[tokenIndex]);

                // Checks if the starting keyword is a valid relative start word
                if (LangDict.TemporalPrepositionSet.Contains(startKeyword))
                {
                    // Extracts first quantity token
                    string firstQuantityToken = tokens[tokenIndex + 1];

                    // Normalizes first unit token
                    string firstUnitToken = LangDict.NormalizeKey(tokens[tokenIndex + 2]);

                    // Normalizes connector token ("and", "et", "y")
                    string connectorToken = LangDict.NormalizeKey(tokens[tokenIndex + 3]);

                    // Extracts second quantity token
                    string secondQuantityToken = tokens[tokenIndex + 4];

                    // Normalizes second unit token
                    string secondUnitToken = LangDict.NormalizeKey(tokens[tokenIndex + 5]);

                    // Validates connector between the two segments
                    bool connectorIsValid = connectorToken == "and" || connectorToken == "et" ||
                        connectorToken == "y";

                    if (connectorIsValid)
                    {
                        // Validates first quantity (word or digit)
                        bool firstQuantityValid = TryParseNumberWord(firstQuantityToken, out int firstQuantity) ||
                            int.TryParse(firstQuantityToken, out firstQuantity);

                        // Validates second quantity (word or digit)
                        bool secondQuantityValid = TryParseNumberWord(secondQuantityToken, out int secondQuantity) ||
                            int.TryParse(secondQuantityToken, out secondQuantity);

                        // Validates first unit ("day", "week", "month", "year")
                        string? firstCanonicalUnit = null;

                        if (LangDict.DayKeywordSet.Contains(firstUnitToken))
                        {
                            firstCanonicalUnit = "day";
                        }

                        else if (LangDict.WeekKeywordSet.Contains(firstUnitToken))
                        {
                            firstCanonicalUnit = "week";
                        }

                        else if (LangDict.MonthKeywordSet.Contains(firstUnitToken))
                        {
                            firstCanonicalUnit = "month";
                        }

                        else if (LangDict.YearKeywordSet.Contains(firstUnitToken))
                        {
                            firstCanonicalUnit = "year";
                        }

                        bool firstUnitValid = firstCanonicalUnit != null;

                        // Validates second unit
                        string? secondCanonicalUnit = null;

                        if (LangDict.DayKeywordSet.Contains(secondUnitToken))
                        {
                            secondCanonicalUnit = "day";
                        }

                        else if (LangDict.WeekKeywordSet.Contains(secondUnitToken))
                        {
                            secondCanonicalUnit = "week";
                        }

                        else if (LangDict.MonthKeywordSet.Contains(secondUnitToken))
                        {
                            secondCanonicalUnit = "month";
                        }

                        else if (LangDict.YearKeywordSet.Contains(secondUnitToken))
                        {
                            secondCanonicalUnit = "year";
                        }

                        bool secondUnitValid = secondCanonicalUnit != null;

                        // Ensures all components are valid
                        if (firstQuantityValid && secondQuantityValid && firstUnitValid && secondUnitValid)
                        {
                            // Temporary interval for first segment
                            DateTime? firstSegmentStart;
                            DateTime? firstSegmentEnd;

                            // Applies first relative offset
                            bool firstRelativeOffsetApplied = TryApplyRelativeUnit(firstQuantity,
                                firstCanonicalUnit!, now, out firstSegmentStart, out firstSegmentEnd);

                            // Continues only if first offset succeeded
                            if (firstRelativeOffsetApplied && firstSegmentStart.HasValue)
                            {
                                // Intermediate date after first offset
                                DateTime intermediateDate = firstSegmentStart.Value;

                                // Temporary interval for second segment
                                DateTime? secondStart;
                                DateTime? secondEnd;

                                // Applies second relative offset
                                bool secondRelativeOffsetApplied = TryApplyRelativeUnit(secondQuantity, secondCanonicalUnit!,
                                    intermediateDate, out secondStart, out secondEnd);

                                // Finalizes result if second offset succeeded
                                if (secondRelativeOffsetApplied && secondStart.HasValue)
                                {
                                    // Composite expression resolves to a single date
                                    startDateTime = secondStart;
                                    endDateTime = secondStart;
                                    parseSuccessful = true;
                                }
                            }
                        }
                    }
                }
            }

            // Pattern: "X unit and Y unit before/after"
            // Examples: "2 weeks and 3 days before", 
            //   "1 month and 2 weeks after", "2 semanas y 3 dias antes"
            // Only evaluate if first pattern failed
            if (!parseSuccessful && tokenIndex + 5 < tokens.Count)
            {
                // Extracts first quantity token
                string firstQuantityToken = tokens[tokenIndex];

                // Normalizes first unit token
                string firstUnitToken = LangDict.NormalizeKey(tokens[tokenIndex + 1]);

                // Normalizes connector token
                string connectorToken = LangDict.NormalizeKey(tokens[tokenIndex + 2]);

                // Extracts second quantity token
                string secondQuantityToken = tokens[tokenIndex + 3];

                // Normalizes second unit token
                string secondUnitToken = LangDict.NormalizeKey(tokens[tokenIndex + 4]);

                // Normalizes direction token ("before", "after", "avant", "despues")
                string directionToken = LangDict.NormalizeKey(tokens[tokenIndex + 5]);

                // Validates connector
                bool connectorIsValid = connectorToken == "and" || connectorToken == "et" ||
                    connectorToken == "y";

                // Validates first quantity
                bool firstQuantityValid = TryParseNumberWord(firstQuantityToken, out int firstQuantity) ||
                    int.TryParse(firstQuantityToken, out firstQuantity);

                // Validates second quantity
                bool secondQuantityValid = TryParseNumberWord(secondQuantityToken, out int secondQuantity) ||
                    int.TryParse(secondQuantityToken, out secondQuantity);

                // Validates first unit
                string? firstCanonicalUnit = null;

                if (LangDict.DayKeywordSet.Contains(firstUnitToken))
                {
                    firstCanonicalUnit = "day";
                }

                else if (LangDict.WeekKeywordSet.Contains(firstUnitToken))
                {
                    firstCanonicalUnit = "week";
                }

                else if (LangDict.MonthKeywordSet.Contains(firstUnitToken))
                {
                    firstCanonicalUnit = "month";
                }

                else if (LangDict.YearKeywordSet.Contains(firstUnitToken))
                {
                    firstCanonicalUnit = "year";
                }

                bool firstUnitValid = firstCanonicalUnit != null;

                // Validates second unit
                string? secondCanonicalUnit = null;

                if (LangDict.DayKeywordSet.Contains(secondUnitToken))
                {
                    secondCanonicalUnit = "day";
                }

                else if (LangDict.WeekKeywordSet.Contains(secondUnitToken))
                {
                    secondCanonicalUnit = "week";
                }

                else if (LangDict.MonthKeywordSet.Contains(secondUnitToken))
                {
                    secondCanonicalUnit = "month";
                }

                else if (LangDict.YearKeywordSet.Contains(secondUnitToken))
                {
                    secondCanonicalUnit = "year";
                }

                bool secondUnitValid = secondCanonicalUnit != null;

                // Validates direction ("before" = -1, "after" = +1)
                bool directionValid = LangDict.TemporalDirectionDict.TryGetValue(directionToken, out int directionSign);

                // Ensures all components are valid
                if (connectorIsValid && firstQuantityValid && secondQuantityValid && firstUnitValid && secondUnitValid &&
                    directionValid)
                {
                    // Applies direction to first quantity
                    int signedFirstQuantity = firstQuantity * directionSign;

                    // Temporary interval for first segment
                    DateTime? firstStart;
                    DateTime? firstEnd;

                    // Applies first offset
                    bool firstApplied = TryApplyRelativeUnit(signedFirstQuantity, firstCanonicalUnit!,
                        now, out firstStart, out firstEnd);

                    // Continues only if first offset succeeded
                    if (firstApplied && firstStart.HasValue)
                    {
                        // Intermediate date after first offset
                        DateTime intermediateDate = firstStart.Value;
                        // Applies direction to second quantity
                        int signedSecondQuantity = secondQuantity * directionSign;

                        // Temporary interval for second segment
                        DateTime? secondStart;
                        DateTime? secondEnd;

                        // Applies second offset
                        bool secondApplied = TryApplyRelativeUnit(signedSecondQuantity, secondCanonicalUnit!,
                            intermediateDate, out secondStart, out secondEnd);

                        // Finalizes result if second offset succeeded
                        if (secondApplied && secondStart.HasValue)
                        {
                            // Composites expression resolves to a single date
                            startDateTime = secondStart;
                            endDateTime = secondStart;
                            parseSuccessful = true;
                        }
                    }
                }
            }

            // Return final parsing status
            return parseSuccessful;
        }

        /// <summary>
        /// Handles simple relative expressions such as:
        /// "in 3 days", "in two weeks", "in 1 month".
        /// </summary>
        private bool TryParseRelativeExpression(List<string> tokens, int tokenIndex, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = null;
            endDateTime = null;

            // Needs preposition + quantity + unit
            if (tokenIndex + 2 >= tokens.Count)
            {
                return false;
            }

            string prepositionToken = LangDict.NormalizeKey(tokens[tokenIndex]);
            string quantityToken = tokens[tokenIndex + 1];
            string unitToken = LangDict.NormalizeKey(tokens[tokenIndex + 2]);

            bool prepositionValid = LangDict.TemporalPrepositionSet.Contains(prepositionToken);
            bool quantityValid = TryParseNumberWord(quantityToken, out int parsedQuantity) ||
                                 int.TryParse(quantityToken, out parsedQuantity);

            string? canonicalUnit = null;

            if (LangDict.DayKeywordSet.Contains(unitToken))
            {
                canonicalUnit = "day";
            }
            else if (LangDict.WeekKeywordSet.Contains(unitToken))
            {
                canonicalUnit = "week";
            }
            else if (LangDict.MonthKeywordSet.Contains(unitToken))
            {
                canonicalUnit = "month";
            }
            else if (LangDict.YearKeywordSet.Contains(unitToken))
            {
                canonicalUnit = "year";
            }

            if (!prepositionValid || !quantityValid || canonicalUnit == null)
            {
                return false;
            }

            // Applies the relative offset
            if (TryApplyRelativeUnit(parsedQuantity, canonicalUnit, now, out startDateTime, out endDateTime)
                && startDateTime.HasValue)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses high‑level temporal range expressions involving weeks, months,
        /// or years combined with directional keywords such as "next", "last",
        /// or "this".  
        ///
        /// This unified parser handles expressions like:
        /// • "next week", "last week", "this week"
        /// • "next month", "last month", "this month"
        /// • "next year", "last year", "this year"
        /// and all their multilingual equivalents, like 
        /// "semaine prochaine", "mes pasado", etc.
        ///
        /// The method computes the correct start and end dates for the requested
        /// temporal range and returns them as a closed interval.
        /// </summary>
        /// <param name="tokens">
        /// Normalized tokens extracted from the user query.
        /// </param>
        /// <param name="today">
        /// Reference date used to resolve relative temporal expressions.
        /// </param>
        /// <param name="start">
        /// Output: the computed start date of the resolved range, or null if no match.
        /// </param>
        /// <param name="end">
        /// Output: the computed end date of the resolved range, or null if no match.
        /// </param>
        /// <returns>
        /// True if the query matches a week/month/year range expression;  
        /// false otherwise.
        /// </returns>

        private bool TryParseTemporalRange(List<string> tokens, DateTime today,
        out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = endDateTime = null;

            bool hasWeek = tokens.Any(token => LangDict.WeekKeywordSet.Contains(token));
            bool hasMonth = tokens.Any(token => LangDict.MonthKeywordSet.Contains(token));
            bool hasYear = tokens.Any(token => LangDict.YearKeywordSet.Contains(token));

            if (!hasWeek && !hasMonth && !hasYear)
            {
                return false;
            }

            bool hasNext = tokens.Any(token => LangDict.TemporalDirectionDict
            .TryGetValue(token, out int parsedDirection) && parsedDirection > 0);

            bool hasLast = tokens.Any(token => LangDict.TemporalDirectionDict
            .TryGetValue(token, out int parsedDirection) && parsedDirection < 0);

            bool hasThis = tokens.Any(token => LangDict.TemporalDirectionDict
            .TryGetValue(token, out int parsedDirection) && parsedDirection == 0);

            // WEEK RANGE
            if (hasWeek)
            {
                int deltaToMonday = ((int)today.DayOfWeek + 6) % 7;
                DateTime mondayThisWeek = today.Date.AddDays(-deltaToMonday);

                if (hasThis)
                {
                    startDateTime = mondayThisWeek;
                    endDateTime = mondayThisWeek.AddDays(6);
                    return true;
                }

                if (hasNext)
                {
                    DateTime mondayNext = mondayThisWeek.AddDays(7);
                    startDateTime = mondayNext;
                    endDateTime = mondayNext.AddDays(6);
                    return true;
                }

                if (hasLast)
                {
                    DateTime mondayLast = mondayThisWeek.AddDays(-7);
                    startDateTime = mondayLast;
                    endDateTime = mondayLast.AddDays(6);
                    return true;
                }
            }

            // MONTH RANGE
            if (hasMonth)
            {
                int year = today.Year;
                int month = today.Month;

                if (hasThis)
                {
                    startDateTime = new DateTime(year, month, 1);
                    endDateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    return true;
                }

                if (hasNext)
                {
                    month++;
                    if (month > 12) { month = 1; year++; }
                    startDateTime = new DateTime(year, month, 1);
                    endDateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    return true;
                }

                if (hasLast)
                {
                    month--;
                    if (month < 1) { month = 12; year--; }
                    startDateTime = new DateTime(year, month, 1);
                    endDateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    return true;
                }
            }

            // YEAR RANGE
            if (hasYear)
            {
                int year = today.Year;

                if (hasThis)
                {
                    startDateTime = new DateTime(year, 1, 1);
                    endDateTime = new DateTime(year, 12, 31);
                    return true;
                }

                if (hasNext)
                {
                    year++;
                    startDateTime = new DateTime(year, 1, 1);
                    endDateTime = new DateTime(year, 12, 31);
                    return true;
                }

                if (hasLast)
                {
                    year--;
                    startDateTime = new DateTime(year, 1, 1);
                    endDateTime = new DateTime(year, 12, 31);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Matches year‑range expressions such as:
        /// "this year", "next year", "last year"
        /// in any supported language.
        /// </summary>
        /// <param name="tokensSet">Set of normalized tokens extracted from the query.</param>
        /// <param name="tokenIndex">Index of the potential start of the pattern.</param>
        /// <param name="now">Reference date used as the anchor for relative calculations.</param>
        /// <param name="startDateTime">Resolved start date if parsing succeeds.</param>
        /// <param name="endDateTime">Resolved end date if parsing succeeds.</param>
        /// <returns>True if a valid year‑range expression is detected.</returns>
        private bool TryParseYearExpression(HashSet<string> tokensSet, int tokenIndex, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = endDateTime = null;

            string? rangeType = null;

            // Scans all tokens
            foreach (string rawToken in tokensSet)
            {
                string normalizedToken = LangDict.NormalizeKey(rawToken);

                // Checks whether the current token expresses a temporal direction
                // If found, the direction is mapped to a normalized range type:
                //   +1 → "next"
                //    0 → "this"
                //   -1 → "last"
                // This allows language‑agnostic handling of year expressions.
                if (LangDict.TemporalDirectionDict.TryGetValue(normalizedToken, out int parsedDirection))
                {
                    rangeType = parsedDirection > 0 ? "next" : parsedDirection < 0 ? "last" : "this";
                    break;
                }
            }

            // No year‑range keyword found
            if (rangeType == null)
            {
                return false;
            }

            int targetYear;

            // Determines the target year based on the range type
            switch (rangeType)
            {
                case "this":
                    targetYear = now.Year;
                    break;

                case "next":
                    targetYear = now.Year + 1;
                    break;

                case "last":
                    targetYear = now.Year - 1;
                    break;

                default:
                    targetYear = now.Year;
                    break;
            }

            // Builds the full year interval
            startDateTime = new DateTime(targetYear, 1, 1);
            endDateTime = new DateTime(targetYear, 12, 31);

            return true;
        }

        /// <summary>
        /// Matches weekday expressions such as:
        /// "next monday" or "last friday" in any supported language.
        /// </summary>
        private bool TryParseWeekdayExpression(HashSet<string> tokensSet, int tokenIndex, DateTime now,
            out DateTime? startDateTime, out DateTime? endDateTime)
        {
            startDateTime = endDateTime = null;

            DayOfWeek? parsedWeekday = null;
            int? parsedDirection = null;

            // Finds a weekday in the token set
            foreach (string rawToken in tokensSet)
            {
                string normalizedToken = LangDict.NormalizeKey(rawToken);

                if (LangDict.WeekdayDict.TryGetValue(normalizedToken, out DayOfWeek foundDayOfWeek))
                {
                    parsedWeekday = foundDayOfWeek;
                    break;
                }
            }

            if (!parsedWeekday.HasValue)
            {
                return false;
            }

            // Rejects expressions like "next week": "week" is a unit, not a weekday.
            // These belong to week‑range parser.
            foreach (string rawToken in tokensSet)
            {
                string normalizedToken = LangDict.NormalizeKey(rawToken);

                if (LangDict.WeekKeywordSet.Contains(normalizedToken))
                {
                    return false;
                }
            }

            // Finds a temporal direction ("next", "last", etc.)
            foreach (string rawToken in tokensSet)
            {
                string normalizedToken = LangDict.NormalizeKey(rawToken);

                if (LangDict.TemporalDirectionDict.TryGetValue(normalizedToken, out int foundDirection))
                {
                    parsedDirection = foundDirection; // +1 or -1
                    break;
                }
            }

            // Computes delta days
            int currentDayNumber = (int)now.DayOfWeek;
            int targetDayNumber = (int)parsedWeekday.Value;

            int deltaDays = targetDayNumber - currentDayNumber;

            if (parsedDirection.HasValue)
            {
                if (parsedDirection.Value == +1) // next
                {
                    if (deltaDays <= 0)
                    {
                        deltaDays += 7;
                    }
                }
                else if (parsedDirection.Value == -1) // last
                {
                    if (deltaDays >= 0)
                    {
                        deltaDays -= 7;
                    }
                }
            }

            DateTime resultDateTime = now.Date.AddDays(deltaDays);

            startDateTime = resultDateTime;
            endDateTime = resultDateTime;
            return true;
        }
    }
}

