using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLinter.Rules
{
    public interface IRule
    {
        string Id { get; }
        Results.LintResult Analyze(string filePath);
    }
}
