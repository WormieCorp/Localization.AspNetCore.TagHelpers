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
	$SrcParameters+=@("-f", "net451")
	$TestParameters+=@("-f", "net451")
}
elseif ($NetCore -and !$NetFull) {
	$SrcParameters+=@("-f", "netstandard1.6")
	$TestParameters+=@("-f", "netcoreapp1.0")
}


dotnet $SrcParameters src/Localization.AspNetCore.TagHelpers/project.json
if ($NetCore) {
	dotnet $TestParameters src/Localization.Demo/project.json
	$NPM_EXEC = Get-Command npm -ErrorAction SilentlyContinue
	if ($NPM_EXEC) {
		pushd src/Localization.Demo
		. $NPM_EXEC run gulp
		popd
	}
}

dotnet $TestParameters test/Localization.AspNetCore.TagHelpers.Tests/project.json
