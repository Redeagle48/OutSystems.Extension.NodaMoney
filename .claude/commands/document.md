Run the odc-docs-writer agent to produce technical documentation for an ODC External Library extension, an agent prompt file, or any knowledge artifact in this workspace.

The agent will:
1. Read the existing project or agent files to understand the current state
2. Identify documentation gaps — missing XML comments, undocumented parameters, stale README sections, incomplete agent descriptions
3. Produce the requested artifact: XML doc comments, README, API reference, CLAUDE.md, agent spec, or multi-agent orchestration guide
4. Validate that all documented names and types match the actual source code
5. Run `dotnet build` to confirm no compilation errors were introduced (for C# projects)

**What you can ask for:**
- `--xml` — add `///` XML doc comments to all public C# members
- `--readme` — generate or update the project README with Actions, Structures, Installation, and Constraints sections
- `--catalog` — generate two ODC Portal catalog files: `CATALOG_DESCRIPTION.md` (≤ 1000 chars, plain text) and `CATALOG_DOC.md` (300–600 words, easy markdown — what it does, actions table, structures table, how to use, constraints)
- `--agent <name>` — write or improve an agent `.md` prompt file with AOE (Agent Optimization Engine) best practices
- `--pipeline` — produce a multi-agent orchestration doc with pipeline diagram, agent registry, gate logic table, and command index
- `--memory` — audit and update MEMORY.md and memory files for accuracy and relevance

After documentation is complete, use `code-reviewer` if you want a final quality gate, or push to your repository.

$ARGUMENTS
