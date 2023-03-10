using Candoumbe.Pipelines;
using Candoumbe.Pipelines.Components;
using Candoumbe.Pipelines.Components.GitHub;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Codecov;
using Nuke.Common.Utilities.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

[GitHubActions("integration", GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    FetchDepth = 0,
    InvokedTargets = new[] { nameof(IUnitTest.Compile), nameof(IUnitTest.UnitTests), nameof(IPublish.Pack), nameof(IPublish.Publish) },
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
    InvokedTargets = new[] { nameof(IUnitTest.Compile), nameof(Tests), nameof(IPublish.Pack) },
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
    InvokedTargets = new[] { nameof(IPublish.Pack), nameof(IPublish.Publish), nameof(ICreateGithubRelease.AddGithubRelease) },
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
    IPublish,
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
    IEnumerable<Project> IMutationTest.MutationTestsProjects => this.Get<IUnitTest>().UnitTestsProjects;

    ///<inheritdoc/>
    Configure<Arguments> IMutationTest.StrykerArgumentsSettings => args => args.Add("--output {0}", this.Get<IMutationTest>().MutationTestResultDirectory);

    ///<inheritdoc/>
    IEnumerable<AbsolutePath> IPack.PackableProjects => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobFiles("**/*.csproj");

    ///<inheritdoc/>
    IEnumerable<PublishConfiguration> IPublish.PublishConfigurations => new PublishConfiguration[]
    {
        new NugetPublishConfiguration(
            apiKey: NugetApiKey,
            source: new Uri("https://api.nuget.org/v3/index.json"),
            canBeUsed: () => NugetApiKey is not null
        ),
        new GitHubPublishConfiguration(
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
