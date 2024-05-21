sealed partial class Build
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    protected override void OnBuildInitialized()
    {
        Configurations =
        [
            "Release*",
            "Installer*"
        ];

        InstallersMap = new()
        {
            {Solution.Installer, Solution.RevitLookup}
        };

        VersionMap = new()
        {
            //{"Release R21", "2021.3.4"},
            //{"Release R22", "2022.3.4"},
            //{"Release R23", "2023.3.4"},
            {"Release R24", "2024.1.4"},
            {"Release R25", "2025.0.4000"}
        };
        // C:\Users\ricau\AppData\Roaming\Autodesk\Revit\Addins\2025
        // dotnet build --configuration="Release R24" -verbosity:m /property:Version=2024.0.0
        // dotnet build --configuration="Release R25" -verbosity:m /property:Version=2025.0.0
    }
}