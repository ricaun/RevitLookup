sealed partial class Build
{
    const string Version = "1.0.1";
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
            { Solution.Installer, Solution.RevitLookup }
        };

        VersionMap = new()
        {
            { "Release R21", Version },
            { "Release R22", Version },
            { "Release R23", Version },
            { "Release R24", Version },
            { "Release R25", Version },
            { "Release R26", Version }
        };

        Bundles = new[]
        {
            Solution.RevitLookup
        };
    }
}