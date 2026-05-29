/// <file>TestRunner.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.2</version>
/// <date>May 29th, 2026</date>

using LifeProManager;
using System;
using System.Collections.Generic;

public static class TestRunner
{
    // This method is called when the user holds CTRL and clicks on the Search button in the UI.
    // It runs a series of 26 tests, each with a different query.
    public static void RunAll(SmartSearch engine)
    {
        Console.WriteLine("=== SMARTSEARCH A to Z TEST RUNNER ===");
        Console.WriteLine("CTRL+clic activated — running 26 tests...\n");

        var tests = new Dictionary<string, string>
        {
            { "A", "demain" },
            { "B", "tomorrow office" },
            { "C", "hier" },
            { "D", "next week" },
            { "E", "dans 3 jours" },
            { "F", "next month" },
            { "G", "next thursday" },
            { "H", "mañana coche" },
            { "I", "kichen today" },
            { "J", "burau demain" },
            { "K", "factura lunes" },
            { "L", "thursday" },
            { "M", "azucar" },
            { "N", "day after tomorrow" },
            { "O", "le mois passé" },
            { "P", "ce mois" },
            { "Q", "mois suivant" },
            { "R", "année prochaine" },
            { "S", "année passée" },
            { "T", "2026" },
            { "U", "last monday" },
            { "V", "año próximo" },
            { "W", "next year inspection" },
            { "X", "declaración pasada" },
            { "Y", "important" },
            { "Z", "anniversaire" }
        };

        foreach (var keyValuePair in tests)
        {
            string testId = keyValuePair.Key;
            string query = keyValuePair.Value;

            List<Tasks> taskResults = engine.Search(query);

            bool taskResultsIsZero = taskResults.Count == 0;

            // Red if no result
            if (taskResultsIsZero)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{testId} | \"{query}\" → 0 result(s) ❌");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{testId} | \"{query}\" → {taskResults.Count} result(s)");
                Console.ResetColor();

                // Display titles
                foreach (var task in taskResults)
                {
                    Console.WriteLine($"   - {task.Title}");
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine("=== END OF TESTS ===");
    }
}
