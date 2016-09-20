param(
	[ValidateSet("Debug","Release")]
	[string]$Configuration,
	[bool]$NetFull,
	[bool]$NetCore,
	[string]$VersionSuffix
)

$SrcParameters=@("build", "-c", "$Configuration", "--no-dependencies")

if ($VersionSuffix -ne "") {
	$SrcParameters+= @("--version-suffix", $VersionSuffix)
}
$TestParameters=$SrcParameters

if ($NetFull -and !$NetCore) {
	$SrcParameters+=@("net451")
	$TestParameters+=@("net451")
}
elseif ($NetCore -and !$NetFull) {
	$SrcParameters+=@("-f", "netstandard1.6")
	$TestFramework+=@("-f", "netcoreapp1.0")
}


dotnet $SrcParameters src/Localization.AspNetCore.TagHelpers/project.json
dotnet $TestParameters test/Localization.AspNetCore.TagHelpers.Tests/project.json
