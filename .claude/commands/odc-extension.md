Run the outsystems-external-code agent to build a complete OutSystems ODC External Library.

The agent will:
1. Clarify the functionality needed (actions, structures, inputs/outputs)
2. Design the interface and confirm it with you before writing any code
3. Generate the complete project: solution, .csproj, interface, implementation, structures, unit tests, GitHub CI/CD workflow, .gitignore, and README
4. Build with `dotnet build` to verify compilation
5. Run `dotnet test` to verify correctness
6. Provide packaging and deployment instructions

Pass any relevant context as arguments — e.g. the NuGet package to wrap, a description of the actions you need, or the target repo/folder.

Once the extension is built, use `/add-telemetry` to instrument it with structured logging and observability via the ODC Flight Recorder SDK.

$ARGUMENTS
