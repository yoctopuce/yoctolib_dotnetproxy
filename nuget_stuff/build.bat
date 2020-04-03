cd @ECHO OFF
if "%VCINSTALLDIR%"=="" call "%VS150COMNTOOLS%vsvars32.bat"
if "%VCINSTALLDIR%"=="" call "%VS140COMNTOOLS%vsvars32.bat"

cd core20
dotnet clean
dotnet build -c Release

cd ..\net45
dotnet clean
dotnet build -c Release

cd ..\pkg
rd /s /q lib
rd /s /q ydll
rd /s /q contentFiles
del *.nupkg
md lib\netstandard2.0\amd64
md lib\net45\
md ydll\amd64


copy ..\..\Sources\dll\*.dll ydll
copy ..\..\Sources\dll\*.so ydll
copy ..\..\Sources\dll\*.dylib ydll
copy ..\..\Sources\dll\amd64\*.* ydll\amd64

copy ..\core20\bin\Release\netstandard2.0\* lib\netstandard2.0\
copy ..\net45\bin\Release\* lib\net45\

nuget pack
cd ..
