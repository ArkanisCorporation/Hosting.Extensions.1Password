/// <summary>
/// Husky.NET commit-msg check: rejects commit subjects that are not Conventional Commits.
/// </summary>
/// <remarks>
/// semantic-release derives the next version from commit types, so a malformed subject either
/// cuts no release or the wrong one. Git-generated subjects (merges, reverts, fixup/squash/amend
/// for autosquash) are allowed through unchanged. Args[0] is the commit message file git passes
/// to the commit-msg hook.
/// </remarks>
using System.Text.RegularExpressions;

// First non-empty, non-comment line is the subject (git strips '#' lines after the hook runs).
var subject = string.Empty;
foreach (var raw in File.ReadAllLines(Args[0]))
{
    var line = raw.Trim();
    if (line.Length > 0 && !line.StartsWith("#"))
    {
        subject = line;
        break;
    }
}

const string conventional = @"^(build|chore|ci|docs|feat|fix|perf|refactor|revert|style|test)(\([^()\r\n]+\))?!?: \S.*$";
const string gitGenerated = @"^(Merge |Revert ""|(fixup|squash|amend)! )";

if (Regex.IsMatch(subject, conventional) || Regex.IsMatch(subject, gitGenerated))
{
    return 0;
}

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine($"Invalid commit subject: \"{subject}\"");
Console.ResetColor();
Console.WriteLine("Use Conventional Commits: <type>(<optional scope>)!: <description>");
Console.WriteLine("Types: build, chore, ci, docs, feat, fix, perf, refactor, revert, style, test");
Console.WriteLine("See https://www.conventionalcommits.org/");
return 1;
