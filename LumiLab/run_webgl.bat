@echo off
title LumiLab - WebGL Showroom Server
echo ========================================================
echo    LumiLab - Interactive 3D Lamp Design Studio (WebGL)
echo ========================================================
echo.
echo Starting local web server on http://localhost:8080/index.html ...
start "" "http://localhost:8080/index.html"
cd /d "%~dp0Builds\WebGL"
python -m http.server 8080
pause

