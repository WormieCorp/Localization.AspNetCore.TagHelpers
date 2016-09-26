$DOTNET=Get-Command -Name dotnet
. $DOTNET restore

$NPM=Get-Command -Name "npm" -ErrorAction SilentlyContinue

if ($NPM) {
	pushd src/Localization.Demo
	. $NPM --loglevel=error install
	popd
}
