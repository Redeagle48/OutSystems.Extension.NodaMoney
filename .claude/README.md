# ODC External Library — Claude Agents Orchestration

A multi-agent workspace for building, instrumenting, reviewing, securing, and documenting **OutSystems Developer Cloud (ODC) External Libraries** in C# .NET 8. Each agent is a specialist. Commands wire them together into a complete delivery pipeline.

---

## Prerequisites

- [Claude Code CLI](https://claude.ai/code) installed and authenticated
- .NET 8 SDK installed (`dotnet --version`)
- This folder open as your Claude Code working directory

---

## Workspace Structure

```
ODC Code Extension Claude Agents/
├── agents/                        ← Agent definitions (one specialist per file)
│   ├── outsystems-pro.md          → outsystems-external-code
│   ├── flight-recorder.md         → odc-flight-recorder
│   ├── code-reviewer.md           → code-reviewer
│   ├── security-auditor.md        → security-auditor
│   ├── penetration-tester.md      → penetration-tester
│   └── docs-writer.md             → odc-docs-writer
├── commands/                      ← Slash commands (type these in Claude Code)
│   ├── odc-extension.md           → /odc-extension
│   ├── add-telemetry.md           → /add-telemetry
│   ├── review.md                  → /review
│   ├── audit.md                   → /audit
│   ├── pentest.md                 → /pentest
│   ├── document.md                → /document
│   └── ship.md                    → /ship
└── settings.local.json            ← Pre-approved tool permissions
```

---

## Agents

| Agent | File | Role |
|-------|------|------|
| `outsystems-external-code` | `outsystems-pro.md` | Builds complete ODC External Library projects from scratch: interface, implementation, structs, unit tests, GitHub CI/CD, packaging |
| `odc-flight-recorder` | `flight-recorder.md` | Instruments existing extensions with structured telemetry via the ODC Flight Recorder SDK — adds `flightPath` and `goldenThreadId` output parameters |
| `code-reviewer` | `code-reviewer.md` | Reviews code quality, ODC constraints, test coverage, NuGet vulnerabilities, naming, and complexity |
| `security-auditor` | `security-auditor.md` | Audits secrets handling, CVEs, CloudWatch hygiene, input validation, and compliance (SOC 2, OWASP, Lambda best practices) |
| `penetration-tester` | `penetration-tester.md` | Tests OSAction injection vectors, dependency chain exploits, Lambda environment exposure, and telemetry data leakage |
| `odc-docs-writer` | `docs-writer.md` | Produces XML doc comments, README files, ODC Portal catalog documents (`CATALOG_DESCRIPTION.md` ≤ 1000 chars and `CATALOG_DOC.md` 300–600 words), agent prompt specs, and AI-agent-optimized knowledge artifacts using AOE patterns |

---

## Commands

### Building

#### `/odc-extension <description>`
Build a complete ODC External Library project from scratch.

```
/odc-extension wrap the libphonenumber-csharp NuGet package with actions for parsing and validating phone numbers
```

The agent clarifies what you need, designs the interface, confirms it with you, then generates the full project — solution, `.csproj`, interface, implementation, structs, unit tests, GitHub workflows, `.gitignore`, and README. Finishes with `dotnet build` and `dotnet test`.

---

#### `/add-telemetry <project-path>`
Add structured telemetry to an existing extension using the ODC Flight Recorder SDK.

```
/add-telemetry ~/projects/OutSystems.Extension.PhoneValidator
```

Adds `ODCFlightRecorder.SDK` NuGet, instruments each action with `FlightRecorder` checkpoints, and adds `flightPath` (structured JSON) and `goldenThreadId` (distributed trace ID) output parameters. Updates unit tests and provides the ODC entity schema for storing telemetry.

---

### Quality & Security Gates

#### `/review <project-path>`
Run a code quality review.

```
/review ~/projects/OutSystems.Extension.PhoneValidator
```

Checks code quality, ODC-specific constraints (stateless Lambda, `[OSStructure]` field types, type mapping), test coverage, and NuGet vulnerability scan. Reports findings as CRITICAL / HIGH / MEDIUM / LOW.

**Gate:** CRITICAL finding → must fix before proceeding. Target score ≥ 85%.

---

#### `/audit <project-path>`
Run a security audit.

```
/audit ~/projects/OutSystems.Extension.PhoneValidator focus on secrets and NuGet CVEs
```

Audits secrets handling, NuGet dependencies, CloudWatch log hygiene, input validation on OSAction parameters, and compliance against SOC 2 and AWS Lambda security controls.

**Gate:** CRITICAL finding → blocks deployment. ≥ 3 HIGH findings → fix recommended. Target compliance ≥ 90%.

---

#### `/pentest <project-path>`
Run a penetration test.

```
/pentest ~/projects/OutSystems.Extension.PhoneValidator
```

Tests OSAction injection vectors (command injection, path traversal), NuGet dependency chain CVEs, Lambda environment variable exposure, and Flight Recorder telemetry for PII or secrets leakage.

**Gate:** Validated CRITICAL exploit → blocks deployment. ≥ 5 MEDIUM findings → triggers a fix loop back through `/review`.

> **Scope:** Limited to ODC-relevant attack surfaces. WiFi, social engineering, and physical access tests are explicitly out of scope.

---

### Documentation

#### `/document <project-path> [flags]`
Generate or update technical documentation.

```
/document ~/projects/OutSystems.Extension.PhoneValidator --xml --readme
```

| Flag | What it produces |
|------|-----------------|
| `--xml` | `///` XML doc comments on all public C# members |
| `--readme` | README with Actions table, Structures table, Installation, Telemetry, Constraints |
| `--catalog` | Two ODC Portal catalog files: `CATALOG_DESCRIPTION.md` (≤ 1000 chars, plain text — paste into the ODC Portal description field) and `CATALOG_DOC.md` (300–600 words, easy markdown — paste into the ODC Portal documentation tab) |
| `--agent <name>` | Agent `.md` prompt file with AOE (Agent Optimization Engine) best practices |
| `--pipeline` | Multi-agent orchestration doc with pipeline diagram, agent registry, gate logic table |
| `--memory` | Audit and update `MEMORY.md` and memory files for accuracy |

---

### Full Pipeline

#### `/ship <project-path>`
Run all four quality and security gates in sequence, then produce documentation.

```
/ship ~/projects/OutSystems.Extension.PhoneValidator
```

This is the command to run when you're ready to deliver. It chains all four stages:

```
Stage 1 → /review    code-reviewer       CRITICAL blocks │ score < 85% blocks
Stage 2 → /audit     security-auditor    CRITICAL blocks │ ≥3 HIGH warns
Stage 3 → /pentest   penetration-tester  CRITICAL blocks │ ≥5 MEDIUM fix loop
Stage 4 → /document  odc-docs-writer     build must pass │ all members documented │ catalog files verified
```

Each stage reports its findings before moving to the next. Any gate failure stops the pipeline and tells you exactly what to fix. When all four stages pass, the extension is ready to package and upload to ODC Portal.

---

## Typical Workflows

### Build a new extension end-to-end

```
1. /odc-extension <what you need>        ← build it
2. /add-telemetry <project-path>         ← instrument it
3. /ship <project-path>                  ← review, audit, pentest, document
```

### Fix an existing extension and re-validate

```
1. /review <project-path>                         ← see what needs fixing
2. [fix the issues yourself or ask Claude]
3. /audit <project-path>                          ← security check
4. /document <project-path> --xml --readme --catalog
```

### Update documentation only

```
/document <project-path> --xml --readme --catalog
```

### Publish to ODC Portal catalog

```
1. /document <project-path> --catalog             ← generates CATALOG_DESCRIPTION.md and CATALOG_DOC.md
2. Copy CATALOG_DESCRIPTION.md → ODC Portal → External Libraries → Description field
3. Copy CATALOG_DOC.md → ODC Portal → External Libraries → Documentation tab
```

### Improve an agent prompt

```
/document --agent docs-writer            ← apply AOE best practices to any agent file
```

---

## Gate Logic Reference

| Stage | Agent | Blocking condition | Warning condition |
|-------|-------|-------------------|------------------|
| Review | `code-reviewer` | Any CRITICAL finding | Score < 85% |
| Audit | `security-auditor` | Any CRITICAL finding | ≥ 3 HIGH findings |
| Pentest | `penetration-tester` | Any validated CRITICAL exploit | ≥ 5 MEDIUM findings (fix loop) |
| Document | `odc-docs-writer` | `dotnet build` fails | Any public member without XML comments; `CATALOG_DESCRIPTION.md` > 1000 chars; `CATALOG_DOC.md` outside 300–600 words |

---

## ODC Deployment (after `/ship` passes)

```bash
dotnet publish -c Release -r linux-x64 --self-contained false
cd bin/Release/net8.0/linux-x64/publish
zip -r ../../../../../OutSystems.Extension.{Name}.zip .
```

Upload the ZIP in **ODC Portal → External Libraries → Upload**.

---

## Adding a New Agent

1. Create `agents/<name>.md` with the frontmatter:
   ```
   ---
   name: agent-identifier
   description: "Use this agent when [precise condition]. NOT for [adjacent wrong use case]."
   tools: Read, Write, Edit, Bash, Glob, Grep
   model: opus
   ---
   ```
2. Create `commands/<command>.md` that invokes it, ending with `$ARGUMENTS`.
3. Update this README.
4. Run `/document --pipeline` to regenerate the orchestration diagram.
