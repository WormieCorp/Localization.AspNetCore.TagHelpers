param(
	[switch]$Help,
	[switch]$Version,
	[switch]$Restore,
	[switch]$Build,
	[switch]$Tests,
	[ValidateSet("Debug", "Release")]
	[string]$Configuration = "Release",
	[ValidateSet("All", "NetFull", "NetCore")]
	[string]$Framework = "All",
	[string]$VersionSuffix = ""
)
$ErrorActionPreference='Stop'

if ($Help) {
"USAGE: build.[cmd|ps1] [OPTIONS]

OPTIONS:
    -Help		Shows this help text
    -Version		Output the version of this build script

    -Configuration	Sets the configuration fo the build
                  	[Possible Values: Debug,Release]
                  	[default: Release]
    -Restore		Enable restoring build packages
    -Build		Build projects
    -Tests		Run Unit Tests

    -Framework		Build using the specified framework
    			[Possible values: All,NetFull,NetCore]
				[default: All]

NOTE:
    if no (Restore|Build|Tests) parameter is supplied, restore, build and unit tests will be run"

	exit 0
}
if ($Version) {
	"build.[cmd|ps1] v.0.1.0"
	exit 0
}

$NetFull = ($Framework -eq "All") -or ($Framework -eq "NetFull")
$NetCore = ($Framework -eq "All") -or ($Framework -eq "NetCore")

if (!$Restore -and !$Build -and !$Tests) {
	$Restore = $true
	$Build = $true
	$Tests = $true
}

echo "RESTORE: $Restore"
echo "BUILD: $Build"
echo "TESTS: $Tests"
echo ".NET Full: $NetFull"
echo ".NET Core: $NetCore"
echo "CONFIGURATION: $Configuration"

pushd "$PSScriptRoot"

if ($Restore) {
	Write-Host "Restoring packages" -Foreground "cyan"
	./.build/restore.ps1
}

if ($Build) {
	Write-Host "Building projects" -Foreground "cyan";
	./.build/build.ps1 -Configuration $Configuration -NetFull $NetFull -NetCore $NetCore -VersionSuffix $VersionSuffix
}

if ($Tests) {
	Write-Host "Running unit tests"
	./.build/tests.ps1 -Configuration $Configuration -NetFull $NetFull -NetCore $NetCore
}

popd
