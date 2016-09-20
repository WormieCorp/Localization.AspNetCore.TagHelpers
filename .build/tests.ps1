param(
	[ValidateSet("Debug","Release")]
	[string]$Configuration,
	[bool]$NetFull,
	[bool]$NetCore
)

if (!(Test-Path test-results)) {
	New-Item test-results -type directory
}

if ($NetCore) {
	dotnet test -c $Configuration -f netcoreapp1.0 test/Localization.AspNetCore.TagHelpers.Tests --no-build
	mv TestResult.xml test-results/TestResult-netcore.xml -Force
}
if ($NetFull) {
	dotnet test -c $Configuration -f net451 test/Localization.AspNetCore.TagHelpers.Tests --no-build
	mv TestResult.xml test-results/TestResult-net451.xml -Force
}
