@echo off
md c:\logs
md c:\data
del /f /q c:\logs\*.log
del /f /q /s c:\data\*.*
rem copy /y /l config.txt c:\
%cd%\bin\mongod --config %cd%\config.txt --install --serviceName mongod --serviceDisplayName mongod
sc start mongod