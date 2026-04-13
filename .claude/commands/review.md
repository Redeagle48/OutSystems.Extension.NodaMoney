Run the code-reviewer agent to conduct a comprehensive code review of an OutSystems ODC External Library extension.

The agent will:
1. Read all source files — interface, implementation, structures, unit tests, and project file
2. Scan for NuGet vulnerabilities with `dotnet list package --vulnerable`
3. Review code quality: logic correctness, error handling, naming, complexity, duplication
4. Review ODC-specific concerns: stateless Lambda constraints, OSStructure field types, OSAction parameter mapping, type safety
5. Review test coverage and quality
6. Produce a structured findings report: CRITICAL, HIGH, MEDIUM, LOW, and suggestions

**Gate logic:**
- Any CRITICAL finding → blocks progression; must be fixed before moving on
- Score reported as a percentage; target ≥ 85% before proceeding to security audit

**Typical next step:** `/audit` for security compliance, or `/document` if score is already ≥ 85% and no blockers.

Pass the path to the project folder, a specific file to review, or a description of what to focus on.

$ARGUMENTS
