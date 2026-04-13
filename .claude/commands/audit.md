Run the security-auditor agent to conduct a security audit and compliance assessment of an OutSystems ODC External Library extension.

The agent will:
1. Read all source files, configuration, project file, and GitHub workflow
2. Audit secrets handling — no hardcoded credentials, API keys, or connection strings in code or workflow files
3. Audit NuGet dependency hygiene — known CVEs, license compliance, transitive dependencies
4. Audit Lambda/CloudWatch hygiene — no sensitive data in log output, proper structured logging
5. Review input validation, authentication checks, and injection attack surfaces on OSAction parameters
6. Assess compliance against relevant controls (SOC 2, OWASP, AWS Lambda security best practices)
7. Produce a findings report: CRITICAL, HIGH, MEDIUM, LOW, with remediation guidance for each

**Gate logic:**
- Any CRITICAL finding → blocks deployment; must be remediated before proceeding
- ≥ 3 HIGH findings → recommended fix loop before proceeding
- Compliance score reported; target ≥ 90% for production deployment

**Typical next step:** `/pentest` for attack surface validation, or `/document` if all gates pass.

Pass the path to the project folder, or specific areas to focus the audit (e.g. "focus on secrets and NuGet CVEs").

$ARGUMENTS
