param(
    [Parameter(Mandatory = $true)]
    [Version]$Version,
    [int]$Build = 0
)
$utf8Encoding = New-Object System.Text.UTF8Encoding($false)
$git = Get-Command git -ErrorAction SilentlyContinue
$changeLogFile = Resolve-Path "$PSScriptRoot/../CHANGELOG.md"
$projectFile = Resolve-Path "$PSScriptRoot/../src/Localization.AspNetCore.TagHelpers/project.json"

$content = Get-Content $changeLogFile
$releaseNotes = ""

$previousNotFixed = $false

$notFixed = ""
$indexesToRemove = New-Object System.Collections.Generic.List[System.Int32]

foreach ($line in $content) {
	if ("$line" -match "#+ UPCOMING\s*") {
		if ($Build -gt 0) {
			$content = $content -replace $Matches[0],"# Version: $Version, Build: $Build"
		} else {
			$content = $content -replace $Matches[0],"# Version: $Version"
		}
		$previousNotFixed = $false;

	} elseif ("$line" -match "\*\s+\[(X|x| )\]\s+(.+)") {
		if ($Matches[1] -ne " ") {
			if (!$Matches[2].StartsWith("Demo:")) {
				$releaseNotes += "* $($Matches[2])\n"
			}
			$previousNotFixed = $false;
		} else {
			$notFixed += "$line`n"
			$previousNotFixed = $true;
			$index = $content.IndexOf($line);
			if ($index -gt -1) {
				$indexesToRemove.Add($index);
			}
		}
	} elseif ("$line" -match "#+.*") {
		break
	} elseif ("$line" -match "\s+.*") {
		if ($previousNotFixed) {
			$notFixed += "$line`n";
		} else {
			$releaseNotes += "$line\n"
		}
	}
}

$contentList = New-Object System.Collections.Generic.List[System.Object]
$contentList.AddRange($content);

foreach ($index in $indexesToRemove) {
	$contentList.RemoveAt($index);
}

if ("$notFixed" -ne "") {
	$newContent = "# UPCOMING`n`n" + $notFixed
	$contentList.Insert(0, $newContent)
}

$projectFileContent = Get-Content -Encoding UTF8 $projectFile
$oldContent = $projectFileContent;

$projectFileContent = $projectFileContent -replace '("releaseNotes"\s*:\s*)".*"',"`$1`"$releaseNotes`""
$projectFileContent = $projectFileContent -replace '("version"\s*:\s*)".*"',"`$1`"$Version-*`""

if ($oldContent -ne $projectFileContent) {
	[System.IO.File]::WriteAllLines($projectFile, $projectFileContent, $utf8Encoding)
}

if ($git) {
	. $git add "$(Resolve-Path $projectFile)"
}

if ($releaseNotes -eq "") {
	Write-Warning "No New Release notes found";
	exit 0
}

$content = $contentList.ToArray();
$content;

[System.IO.File]::WriteAllLines($changeLogFile, $content, $utf8Encoding)

if ($git) {
	. $git add "$(Resolve-Path $changeLogFile)"
	. $git commit -m "Updated Release notes

	[skip ci]"
}
