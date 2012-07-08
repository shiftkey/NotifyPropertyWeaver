param($installPath, $toolsPath, $package, $project)

$path_to_vsix = $Env:LOCALAPPDATA + "\Microsoft\VisualStudio\10.0\Extensions\Simon Cropp\Notify Property Weaver\9.9"
$vsix_exist = test-path -path $path_to_vsix



If (!$vsix_exist) {
	$DirInfo = New-Object System.IO.DirectoryInfo($Env:VS100COMNTOOLS)
	$path = [io.path]::Combine($DirInfo.Parent.FullName, "IDE")
	$path = [io.path]::Combine($path, "VSIXInstaller.exe")
	[Array]$arguments = $toolsPath + "\NotifyPropertyWeaverVsPackage.vsix"
	
	&$path $arguments | out-null

}
uninstall-package NotifyPropertyWeaver -ProjectName $project.Name
$project.Save()
$assemblyPath= $path_to_vsix + "\NotifyPropertyWeaverVsPackage.dll"
Add-Type -Path $assemblyPath


$buildTaskDir = [System.IO.Path]::Combine($project.Object.DTE.Solution.FullName,"..\Tools")

$resourceExporter = New-Object NotifyPropertyWeaverFileExporter
$resourceExporter.ExportTask($buildTaskDir)

$projectInjector = New-Object NotifyPropertyWeaverProjectInjector
$projectInjector.ToolsDirectory = "`$(SolutionDir)Tools"
$projectInjector.ProjectFile = $project.FullName 
$projectInjector.Target = "AfterCompile"
$projectInjector.Execute()



If (!$vsix_exist) {
	"You must restart Microsoft Visual Studio in order for the changes to take effect"
}
