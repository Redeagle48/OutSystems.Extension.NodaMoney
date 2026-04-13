Run the full ODC External Library delivery pipeline in sequence, from code review through security to documentation.

This command orchestrates all quality and security gates in the correct order. Run it on a project that has already been built (`/odc-extension`) and optionally instrumented (`/add-telemetry`).

---

## Pipeline Stages

### Stage 1 — Code Review (`code-reviewer`)
Run: `/review $ARGUMENTS`

What happens:
- Reads all source files and runs `dotnet list package --vulnerable`
- Reviews code quality, ODC-specific constraints, and test coverage
- Produces findings report with severity classification

Gate:
- **CRITICAL found → STOP.** Fix all CRITICAL issues and re-run `/ship` from Stage 1.
- Score < 85% → list blockers and pause for fixes before continuing.
- Score ≥ 85% and zero CRITICAL → proceed to Stage 2.

---

### Stage 2 — Security Audit (`security-auditor`)
Run: `/audit $ARGUMENTS`

What happens:
- Audits secrets handling, NuGet CVEs, CloudWatch hygiene, and input validation
- Maps compliance against SOC 2 controls and AWS Lambda security best practices
- Produces audit report with CRITICAL / HIGH / MEDIUM / LOW findings

Gate:
- **CRITICAL found → STOP.** Remediate and re-run from Stage 1.
- ≥ 3 HIGH findings → recommended fix cycle; confirm with user before proceeding.
- Compliance score ≥ 90% and zero CRITICAL → proceed to Stage 3.

---

### Stage 3 — Penetration Test (`penetration-tester`)
Run: `/pentest $ARGUMENTS`

What happens:
- Tests OSAction injection vectors, dependency chain exploits, Lambda environment exposure
- Validates that Flight Recorder telemetry (if present) does not leak PII or secrets
- Produces pentest report with CVSS scores and proof-of-concept notes

Gate:
- **Any validated CRITICAL exploit → STOP.** Fix and re-run from Stage 1.
- ≥ 5 MEDIUM findings → trigger fix loop with `code-reviewer`, then re-run Stage 3.
- Zero CRITICAL exploits and < 5 MEDIUM → proceed to Stage 4.

---

### Stage 4 — Documentation (`odc-docs-writer`)
Run: `/document --xml --readme --catalog $ARGUMENTS`

What happens:
- Adds `///` XML doc comments to all public C# members
- Generates or updates the README: Actions table, Structures table, Installation, Telemetry, Constraints
- Generates two ODC Portal catalog files:
  - `CATALOG_DESCRIPTION.md` — ≤ 1000 characters, plain text, ready to paste into the ODC Portal description field
  - `CATALOG_DOC.md` — 300–600 words, easy markdown (what it does, actions table, structures table, how to use, constraints), ready to paste into the ODC Portal documentation tab
- Validates all documented parameter names match the actual C# signatures
- Runs `dotnet build` to confirm documentation changes did not break compilation

Gate:
- Build must pass after documentation changes.
- All public members must have XML comments before marking complete.
- `CATALOG_DESCRIPTION.md` must be ≤ 1000 characters with no markdown syntax.
- `CATALOG_DOC.md` must be 300–600 words with no code blocks.

---

## Summary

```
Stage 1: /review    →  code-reviewer       [CRITICAL blocks]
Stage 2: /audit     →  security-auditor    [CRITICAL blocks, ≥3 HIGH warns]
Stage 3: /pentest   →  penetration-tester  [CRITICAL blocks, ≥5 MEDIUM loops]
Stage 4: /document  →  odc-docs-writer     [build must pass │ catalog files verified]
```

At each gate, if a blocker is found the pipeline stops and reports exactly what must be fixed before it can continue. When all four stages pass, the extension is ready to package and deploy to ODC Portal.

---

Pass the path to the project folder as the argument. All four agents will use that path as their working context.

$ARGUMENTS
