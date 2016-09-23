param(
	[string]$Configuration = $true
)
$sevenzip = Get-Command "7z"
$basePath = Resolve-Path "$PSScriptRoot/.."
$artifactPath = "$basePath\artifacts"
$artifactName = "Localization.AspnetCore.TagHelpers"
$exclude = @("*.xproj","*.xproj.user","project.lock.json","bin","obj","wwwroot")

$files = @(
	Get-ChildItem "$PSScriptRoot" -Exclude $MyInvocation.MyCommand.Name
	Get-ChildItem "$basePath" -Include "build","build.ps1","build.cmd"
	Get-ChildItem "$basePath/src/Localization.AspNetCore.TagHelpers" -Exclude $exclude
	Get-ChildItem "$basePath/src/Localization.Demo" -Exclude $exclude
	Get-ChildItem "$basePath/src/Localization.Demo/wwwroot" -Exclude "lib"
	Get-ChildItem "$basePath/test/Localization.AspNetCore.TagHelpers.Tests" -Exclude $exclude
)

$paths = $files | select -ExpandProperty FullName

if (!(Test-Path "$basePath/artifacts")) {
	New-Item -Path "$basePath/artifacts" -ItemType Directory;
} else {
	Remove-Item "$artifactPath/$artifactName.*"
}

#7z a "$basePath/artifacts/Localization.AspnetCore.TagHelpers" $files -r

#$exString = '-x!".git" -x!".vs" -x!".vscode" -x!packages -xr!bin -xr!obj'
$exclusions = @(
	'-x!".git"'
	'-x!".vs"'
	'-x!".vscode"'
	'-x!"*.yml"'
	'-x!".editorconfig"'
	'-x!".git*"'
	'-x!artifacts'
	"-x!packages"
	"-x!Vagrantfile"
	"-xr!bin"
	"-xr!obj"
	"-xr!project.lock.json"
	"-xr!*.xproj*"
	'-x!"src\Localization.Demo\wwwroot\lib"'
	'-x!"*.sln"'
)

7z a "$artifactPath\$artifactName-Source.zip" "$basePath\*" $exclusions
7z a "$artifactPath\$artifactName.tar" "$basePath\*" $exclusions
7z a "$artifactPath\$artifactName-Source.tar.xz" "$artifactPath\$artifactName.tar"
Remove-Item "$artifactPath/$artifactName.tar"
