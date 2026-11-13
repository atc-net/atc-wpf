<#
.SYNOPSIS
Checks that PackageValidationBaselineVersion is the latest release.

.DESCRIPTION
Package validation (dotnet pack) compares the public API of every package with the baseline
version in src/Directory.Build.props. After a release the baseline must be bumped to the version
just published, or API breaks since then go unnoticed. This script fails when the baseline is not
the latest v* tag reachable from the current commit, so CI catches a stale baseline.

.PARAMETER RepoRoot
The repository root. Defaults to the parent of the folder that holds this script.

.EXAMPLE
pwsh tool/Check-PackageValidationBaseline.ps1

.OUTPUTS
None. Exit code 0 when the baseline matches the latest tag, 1 otherwise.
#>
[CmdletBinding()]
param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string] $RepoRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Continue'

$propsPath = Join-Path -Path $RepoRoot -ChildPath 'src/Directory.Build.props'
$props = Get-Content -LiteralPath $propsPath -Raw
if ($props -notmatch '<PackageValidationBaselineVersion>(?<version>[^<]+)</PackageValidationBaselineVersion>') {
    Write-Error -Message "No PackageValidationBaselineVersion found in $propsPath."
    exit 1
}

$baseline = $Matches['version'].Trim()

# The checkout needs the tags (actions/checkout with fetch-depth: 0).
$tag = & git -C $RepoRoot describe --tags --abbrev=0 --match 'v*' 2>$null
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($tag)) {
    Write-Error -Message 'No v* release tag is reachable from this commit; fetch the tags first.'
    exit 1
}

$latest = $tag.Trim().TrimStart('v')

if ($baseline -ne $latest) {
    Write-Error -Message "PackageValidationBaselineVersion is $baseline but the latest release tag is v$latest; bump it in src/Directory.Build.props."
    exit 1
}

Write-Verbose -Message "PackageValidationBaselineVersion $baseline matches the latest release tag v$latest."
Write-Output -InputObject "PackageValidationBaselineVersion $baseline matches v$latest."
exit 0
