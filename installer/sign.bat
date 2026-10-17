@echo off

echo Signing x64 installer...
signtool.exe sign /f ..\BallScanner\aLeeTheY__KeyFile.pfx /p "qwerty123456" /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 ".\output\BallScanner_1.1.0_windows_x64_setup.exe"

echo Signing x86 installer...
signtool.exe sign /f ..\BallScanner\aLeeTheY__KeyFile.pfx /p "qwerty123456" /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 ".\output\BallScanner_1.1.0_windows_x86_setup.exe"

echo Done!
pause
