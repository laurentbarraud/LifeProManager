/// <file>TestRunner.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 5th, 2026</date>

using LifeProManager;
using System;
using System.Collections.Generic;

public static class TestRunner
{
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

        foreach (var kv in tests)
        {
            string testId = kv.Key;
            string query = kv.Value;

            List<Tasks> taskResults = engine.Search(query);

            if (taskResults.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{testId} | \"{query}\" → 0 result(s) ❌");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{testId} | \"{query}\" → {taskResults.Count} result(s)");

                foreach (Tasks t in taskResults)
                {
                    Console.WriteLine($"   - {t.Title}");
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        Console.WriteLine("=== END OF TESTS ===");
    }
}
