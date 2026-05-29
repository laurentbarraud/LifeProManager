/// <file>ScoredTask.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.2</version>
/// <date>May 29th, 2026</date>
/// 
using System;

namespace LifeProManager
{
    /// <summary>
    /// Represents a task associated with its computed SmartSearch relevance score.
    /// This class is used after scoring to sort tasks by descending relevance.
    /// </summary>
    public class ScoredTask
    {
        public Tasks Task { get; set; }
        public int Score { get; set; }
    }
}
