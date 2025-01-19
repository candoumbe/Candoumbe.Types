using System;
using System.Collections.Generic;
using System.Linq;
using Candoumbe.Pipelines.Components;
using Candoumbe.Pipelines.Components.GitHub;
using Candoumbe.Pipelines.Components.NuGet;
using Candoumbe.Pipelines.Components.Workflows;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Codecov;
using Nuke.Common.Tools.GitHub;

[GitHubActions("integration", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    FetchDepth = 0,
    InvokedTargets = [nameof(IUnitTest.Compile), nameof(IUnitTest.UnitTests), nameof(IPushNugetPackages.Pack), nameof(IPushNugetPackages.Publish)],
    CacheKeyFiles =
    [
        "src/**/*.csproj",
        "test/**/*.csproj",
        "stryker-config.json",
        "test/**/*/xunit.runner.json"
    ],
    OnPushBranchesIgnore = [IGitFlowWithPullRequest.MainBranchName],
    EnableGitHubToken = true,
    ImportSecrets =
    [
        nameof(NugetApiKey),
        nameof(IReportCoverage.CodecovToken)
    ],
    PublishArtifacts = true,
    OnPullRequestExcludePaths =
    [
        "docs/*",
        "README.md",
        "CHANGELOG.md",
        "LICENSE"
    ]
)]
[GitHubActions("nightly", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    FetchDepth = 0,
    OnCronSchedule = "0 0 * * *",
    InvokedTargets = [nameof(IMutationTest.MutationTests), nameof(IPushNugetPackages.Pack)],
    OnPushBranches = [IHaveDevelopBranch.DevelopBranchName],
    CacheKeyFiles =
    [
        "src/**/*.csproj",
        "test/**/*.csproj",
        "stryker-config.json",
        "test/**/*/xunit.runner.json"
    ],
    EnableGitHubToken = true,
    ImportSecrets =
    [
        nameof(NugetApiKey),
        nameof(IReportCoverage.CodecovToken),
        nameof(IMutationTest.StrykerDashboardApiKey)
    ],
    PublishArtifacts = true,
    OnPullRequestExcludePaths =
    [
        "docs/*",
        "README.md",
        "CHANGELOG.md",
        "LICENSE"
    ]
)]
[GitHubActions("delivery", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    FetchDepth = 0,
    InvokedTargets = [nameof(IPushNugetPackages.Pack), nameof(IPushNugetPackages.Publish), nameof(ICreateGithubRelease.AddGithubRelease)],
    CacheKeyFiles =
    [
        "src/**/*.csproj",
        "test/**/*.csproj",
        "stryker-config.json",
        "test/**/*/xunit.runner.json"
    ],
    OnPushBranches = [IGitFlowWithPullRequest.MainBranchName],
    EnableGitHubToken = true,
    ImportSecrets =
    [
        nameof(NugetApiKey),
        nameof(IReportCoverage.CodecovToken)
    ],
    PublishArtifacts = true,
    OnPullRequestExcludePaths =
    [
        "docs/*",
        "README.md",
        "CHANGELOG.md",
        "LICENSE"
    ]
)]
[GitHubActions("perf-manual", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    FetchDepth = 0,
    InvokedTargets = [nameof(IBenchmark.Benchmarks)],
    CacheKeyFiles =
    [
        "src/**/*.csproj",
        "test/**/*.csproj",
        "stryker-config.json",
        "test/**/*/xunit.runner.json"
    ],
    On = [GitHubActionsTrigger.WorkflowDispatch],
    EnableGitHubToken = true,
    PublishArtifacts = true
)]
public class Pipelines : EnhancedNukeBuild,
    IHaveSolution,
    IHaveSourceDirectory,
    IHaveTestDirectory,
    IHaveChangeLog,
    IClean,
    IRestore,
    ICompile,
    IUnitTest,
    IBenchmark,
    IHaveGitVersion,
    IReportCoverage,
    IMutationTest,
    IPack,
    IPushNugetPackages,
    ICreateGithubRelease,
    IGitFlowWithPullRequest
{
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
    IEnumerable<AbsolutePath> IClean.DirectoriesToDelete =>
    [
        .. this.Get<IHaveSourceDirectory>().SourceDirectory.GlobDirectories("**/obj", "**/bin"),
        .. this.Get<IHaveTestDirectory>().TestDirectory.GlobDirectories("**/obj", "**/bin")
    ];

    ///<inheritdoc/>
    IEnumerable<AbsolutePath> IClean.DirectoriesToEnsureExistence =>
    [
        this.Get<IHaveArtifacts>().OutputDirectory,
        this.Get<IHaveArtifacts>().ArtifactsDirectory
    ];

    ///<inheritdoc/>
    IEnumerable<Project> IUnitTest.UnitTestsProjects => this.Get<IHaveSolution>().Solution.GetAllProjects("*.UnitTests");

    ///<inheritdoc/>
    IEnumerable<MutationProjectConfiguration> IMutationTest.MutationTestsProjects =>
    [
        new MutationProjectConfiguration(Solution.AllProjects.Single(project => string.Equals(project.Name, "Candoumbe.Types", StringComparison.InvariantCultureIgnoreCase)),
                                         this.Get<IHaveSolution>().Solution.GetAllProjects("*UnitTests"),
                                         this.Get<IHaveTestDirectory>().TestDirectory / "stryker-config.json")
    ];

    ///<inheritdoc/>
    IEnumerable<AbsolutePath> IPack.PackableProjects => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobFiles("**/*.csproj");

    ///<inheritdoc/>
    IEnumerable<PushNugetPackageConfiguration> IPushNugetPackages.PublishConfigurations =>
    [
        new NugetPushConfiguration(
            apiKey: NugetApiKey,
            source: new Uri("https://api.nuget.org/v3/index.json"),
            canBeUsed: () => NugetApiKey is not null
        ),
        new GitHubPushNugetConfiguration(
            githubToken: this.Get<ICreateGithubRelease>()?.GitHubToken,
            source: new Uri($"https://nuget.pkg.github.com/{this.Get<IHaveGitHubRepository>().GitRepository.GetGitHubOwner()}/index.json"),
            canBeUsed: () => this is ICreateGithubRelease createRelease && createRelease.GitHubToken is not null
        )
    ];

    public Target Tests => _ => _
        .TryDependsOn<IUnitTest>(x => x.UnitTests)
        .TryDependsOn<IMutationTest>(x => x.MutationTests)
        .Description("Run both unit and mutation tests")
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
    
    ///<inheritdoc/>
    IEnumerable<Project> IBenchmark.BenchmarkProjects => this.Get<IHaveSolution>().Solution.GetAllProjects("*.PerformanceTests");
}
