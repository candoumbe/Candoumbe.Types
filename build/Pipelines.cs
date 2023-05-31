[GitHubActions("integration", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    FetchDepth = 0,
    InvokedTargets = new[] { nameof(IUnitTest.Compile), nameof(IUnitTest.UnitTests), nameof(IPushNugetPackages.Pack), nameof(IPushNugetPackages.Publish) },
    CacheKeyFiles = new[] {
        "src/**/*.csproj",
        "test/**/*.csproj",
        "stryker-config.json",
        "test/**/*/xunit.runner.json" },
    OnPushBranchesIgnore = new[] { IGitFlowWithPullRequest.MainBranchName },
    EnableGitHubToken = true,
    ImportSecrets = new[]
    {
        nameof(NugetApiKey),
        nameof(IReportCoverage.CodecovToken)
    },
    PublishArtifacts = true,
    OnPullRequestExcludePaths = new[]
    {
        "docs/*",
        "README.md",
        "CHANGELOG.md",
        "LICENSE"
    }
)]
[GitHubActions("nightly", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    FetchDepth = 0,
    OnCronSchedule = "0 0 * * *",
    InvokedTargets = new[] { nameof(IUnitTest.Compile), nameof(Tests), nameof(IPushNugetPackages.Pack) },
    OnPushBranches = new[] { IGitFlowWithPullRequest.DevelopBranchName },
    CacheKeyFiles = new[] {
        "src/**/*.csproj",
        "test/**/*.csproj",
        "stryker-config.json",
        "test/**/*/xunit.runner.json" },
    EnableGitHubToken = true,
    ImportSecrets = new[]
    {
        nameof(NugetApiKey),
        nameof(IReportCoverage.CodecovToken),
        nameof(IMutationTest.StrykerDashboardApiKey)
    },
    PublishArtifacts = true,
    OnPullRequestExcludePaths = new[]
    {
        "docs/*",
        "README.md",
        "CHANGELOG.md",
        "LICENSE"
    }
)]
[GitHubActions("delivery", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    FetchDepth = 0,
    InvokedTargets = new[] { nameof(IPushNugetPackages.Pack), nameof(IPushNugetPackages.Publish), nameof(ICreateGithubRelease.AddGithubRelease) },
    CacheKeyFiles = new[] {
        "src/**/*.csproj",
        "test/**/*.csproj",
        "stryker-config.json",
        "test/**/*/xunit.runner.json" },
    OnPushBranches = new[] { IGitFlowWithPullRequest.MainBranchName },
    EnableGitHubToken = true,
    ImportSecrets = new[]
    {
        nameof(NugetApiKey),
        nameof(IReportCoverage.CodecovToken)
    },
    PublishArtifacts = true,
    OnPullRequestExcludePaths = new[]
    {
        "docs/*",
        "README.md",
        "CHANGELOG.md",
        "LICENSE"
    }
)]
public class Pipelines : NukeBuild,
    IHaveSolution,
    IHaveSourceDirectory,
    IHaveTestDirectory,
    IHaveChangeLog,
    IClean,
    IRestore,
    ICompile,
    IUnitTest,
    IReportCoverage,
    IMutationTest,
    IPack,
    IPushNugetPackages,
    ICreateGithubRelease,
    IGitFlowWithPullRequest,
    IHaveSecret
{

    [CI]
    public GitHubActions GitHubActions;

    [Required]
    [Solution]
    public Solution Solution;

    ///<inheritdoc/>
    Solution IHaveSolution.Solution => Solution;

    /// <summary>
    /// Token used to interact with GitHub API
    /// </summary>
    [Parameter("Token used to interact with Nuget API")]
    [Secret]
    public readonly string NugetApiKey;

    public static int Main() => Execute<Pipelines>(x => ((ICompile)x).Compile);

    ///<inheritdoc/>
    IEnumerable<AbsolutePath> IClean.DirectoriesToDelete => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobDirectories("**/obj", "**/bin")
                                                               .Concat(this.Get<IHaveTestDirectory>().TestDirectory.GlobDirectories("**/obj", "**/bin"));

    ///<inheritdoc/>
    IEnumerable<AbsolutePath> IClean.DirectoriesToEnsureExistance => new[]
    {
        this.Get<IHaveArtifacts>().OutputDirectory,
        this.Get<IHaveArtifacts>().ArtifactsDirectory,
    };

    ///<inheritdoc/>
    IEnumerable<Project> IUnitTest.UnitTestsProjects => this.Get<IHaveSolution>().Solution.GetProjects("*.*Tests");

    ///<inheritdoc/>
    IEnumerable<(Project SourceProject, IEnumerable<Project> TestProjects)> IMutationTest.MutationTestsProjects
        => new[]
        {
            (Source: Solution.GetProject("Candoumbe.Types"), TestProjects: this.Get<IUnitTest>().UnitTestsProjects)
        };

    ///<inheritdoc/>
    IEnumerable<AbsolutePath> IPack.PackableProjects => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobFiles("**/*.csproj");

    ///<inheritdoc/>
    IEnumerable<PushNugetPackageConfiguration> IPushNugetPackages.PublishConfigurations => new PushNugetPackageConfiguration[]
    {
        new NugetPushConfiguration(
            apiKey: NugetApiKey,
            source: new Uri("https://api.nuget.org/v3/index.json"),
            canBeUsed: () => NugetApiKey is not null
        ),
        new GitHubPushNugetConfiguration(
            githubToken: this.Get<ICreateGithubRelease>()?.GitHubToken,
            source: new Uri($"https://nuget.pkg.github.com/{GitHubActions?.RepositoryOwner}/index.json"),
            canBeUsed: () => this is ICreateGithubRelease createRelease && createRelease.GitHubToken is not null
        ),
    };

    public Target Tests => _ => _
        .TryDependsOn<IUnitTest>(x => x.UnitTests)
        .TryDependsOn<IMutationTest>(x => x.MutationTests)
        .Description("Run all tests")
        .Executes(() =>
        {
            // Nothing to set here
        });

    ///<inheritdoc/>
    bool IReportCoverage.ReportToCodeCov => this.Get<IReportCoverage>().CodecovToken is not null;

    ///<inheritdoc/>
    Configure<CodecovSettings> IReportCoverage.CodecovSettings => _ => _.SetFramework("netcoreapp3.1");


    protected override void OnBuildCreated()
    {
        if (IsServerBuild)
        {
            Environment.SetEnvironmentVariable("DOTNET_ROLL_FORWARD", "LatestMajor");
        }
    }
}
