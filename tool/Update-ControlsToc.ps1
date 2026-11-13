<#
.SYNOPSIS
Generates docs/Controls/toc.yml from the readme files under src/.

.DESCRIPTION
The docs site lists every control readme through docs/Controls/toc.yml. This script builds that
file from the *_Readme.md, @Readme.md and Readme.md files of each src/Atc.Wpf* library, so a new
readme shows up on the site without editing the table of contents by hand.

Layout rules:
- Libraries appear in tier order (see $LibraryOrder); a library not in the list follows, alphabetically.
- Each library and folder lists its own readme (Readme.md, README.md or @Readme.md) first, named
  "Overview", then its *_Readme.md files, then its sub folders; files and folders are sorted
  alphabetically (ordinal, ignoring case).
- A folder whose only readme is its own becomes a single entry named after the folder (a library
  always keeps its group with an "Overview" entry).
- A *_Readme.md file is named after the part before "_Readme".

.PARAMETER RepoRoot
The repository root. Defaults to the parent of the folder that holds this script.

.PARAMETER Check
Compares the generated text with docs/Controls/toc.yml instead of writing it; exits 1 when they differ.

.EXAMPLE
pwsh tool/Update-ControlsToc.ps1

.EXAMPLE
pwsh tool/Update-ControlsToc.ps1 -Check

.OUTPUTS
None. Writes docs/Controls/toc.yml, or with -Check exits 0 (up to date) or 1 (stale).
#>
[CmdletBinding()]
param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string] $RepoRoot = (Split-Path -Parent $PSScriptRoot),

    [Parameter()]
    [switch] $Check
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$LibraryOrder = @(
    'Atc.Wpf'
    'Atc.Wpf.Controls'
    'Atc.Wpf.Forms'
    'Atc.Wpf.Components'
    'Atc.Wpf.FontIcons'
    'Atc.Wpf.Theming'
    'Atc.Wpf.Network'
    'Atc.Wpf.Hardware'
    'Atc.Wpf.UndoRedo'
    'Atc.Wpf.Controls.Sample'
)

$Header = '# Generated from the *_Readme.md, @Readme.md and Readme.md files under src/, grouped by library.'
$SkippedFolders = @('bin', 'obj')

function Get-OwnReadme {
    [CmdletBinding()]
    param([Parameter(Mandatory)] [System.IO.DirectoryInfo] $Folder)

    Get-ChildItem -LiteralPath $Folder.FullName -File |
        Where-Object { $_.Name -ieq 'Readme.md' -or $_.Name -eq '@Readme.md' } |
        Select-Object -First 1
}

function Get-RelativeHref {
    [CmdletBinding()]
    param([Parameter(Mandatory)] [System.IO.FileInfo] $File)

    $relative = [System.IO.Path]::GetRelativePath($RepoRoot, $File.FullName).Replace('\', '/')
    "../../$relative"
}

function New-FolderNode {
    <# Builds the node for a folder, or returns $null when the folder holds no readme at any depth. #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)] [System.IO.DirectoryInfo] $Folder,
        [Parameter(Mandatory)] [string] $Name,
        [Parameter()] [switch] $IsLibrary
    )

    $children = [System.Collections.Generic.List[object]]::new()

    $own = Get-OwnReadme -Folder $Folder
    if ($null -ne $own) {
        $children.Add([pscustomobject]@{ Name = 'Overview'; Href = (Get-RelativeHref -File $own); Items = $null })
    }

    $files = Get-ChildItem -LiteralPath $Folder.FullName -File -Filter '*_Readme.md' |
        Sort-Object -Property @{ Expression = { $_.Name.ToUpperInvariant() } }
    foreach ($file in $files) {
        $leafName = $file.Name.Substring(0, $file.Name.Length - '_Readme.md'.Length)
        $children.Add([pscustomobject]@{ Name = $leafName; Href = (Get-RelativeHref -File $file); Items = $null })
    }

    $subFolders = Get-ChildItem -LiteralPath $Folder.FullName -Directory |
        Where-Object { $SkippedFolders -notcontains $_.Name } |
        Sort-Object -Property @{ Expression = { $_.Name.ToUpperInvariant() } }
    foreach ($subFolder in $subFolders) {
        $node = New-FolderNode -Folder $subFolder -Name $subFolder.Name
        if ($null -ne $node) {
            $children.Add($node)
        }
    }

    if ($children.Count -eq 0) {
        return $null
    }

    # A folder with only its own readme is one entry, named after the folder; a library keeps its group.
    if (-not $IsLibrary -and $children.Count -eq 1 -and $null -ne $own) {
        return [pscustomobject]@{ Name = $Name; Href = $children[0].Href; Items = $null }
    }

    [pscustomobject]@{ Name = $Name; Href = $null; Items = $children }
}

function Write-Node {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)] [object] $Node,
        [Parameter(Mandatory)] [int] $Depth,
        [Parameter(Mandatory)] [System.Text.StringBuilder] $Builder
    )

    $indent = '  ' * $Depth
    [void] $Builder.Append("$indent- name: $($Node.Name)`n")
    if ($null -ne $Node.Href) {
        [void] $Builder.Append("$indent  href: $($Node.Href)`n")
    }

    if ($null -ne $Node.Items) {
        [void] $Builder.Append("$indent  items:`n")
        foreach ($child in $Node.Items) {
            Write-Node -Node $child -Depth ($Depth + 1) -Builder $Builder
        }
    }
}

$srcPath = Join-Path -Path $RepoRoot -ChildPath 'src'
$libraries = Get-ChildItem -LiteralPath $srcPath -Directory -Filter 'Atc.Wpf*' |
    Sort-Object -Property @(
        @{ Expression = { $index = $LibraryOrder.IndexOf($_.Name); if ($index -lt 0) { [int]::MaxValue } else { $index } } },
        @{ Expression = { $_.Name.ToUpperInvariant() } }
    )

$builder = [System.Text.StringBuilder]::new()
[void] $builder.Append("$Header`n")
[void] $builder.Append("- name: Overview`n  href: index.md`n")

foreach ($library in $libraries) {
    $node = New-FolderNode -Folder $library -Name $library.Name -IsLibrary
    if ($null -ne $node) {
        Write-Node -Node $node -Depth 0 -Builder $builder
    }
}

$generated = $builder.ToString()
$tocPath = Join-Path -Path $RepoRoot -ChildPath 'docs/Controls/toc.yml'

if ($Check) {
    $current = if (Test-Path -LiteralPath $tocPath) { [System.IO.File]::ReadAllText($tocPath).Replace("`r`n", "`n") } else { '' }
    if ($current -ne $generated) {
        Write-Error -Message 'docs/Controls/toc.yml is out of date; run: pwsh tool/Update-ControlsToc.ps1' -ErrorAction Continue
        exit 1
    }

    Write-Output -InputObject 'docs/Controls/toc.yml is up to date.'
    exit 0
}

# Keep the line endings the checkout already uses (Git converts them on commit).
if ((Test-Path -LiteralPath $tocPath) -and [System.IO.File]::ReadAllText($tocPath).Contains("`r`n")) {
    $generated = $generated.Replace("`n", "`r`n")
}

[System.IO.File]::WriteAllText($tocPath, $generated, [System.Text.UTF8Encoding]::new($false))
Write-Output -InputObject "Wrote $tocPath."
