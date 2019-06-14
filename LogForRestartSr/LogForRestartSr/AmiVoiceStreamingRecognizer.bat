@echo off

rem *********************
rem 服務參數設定
rem *********************

rem 服務器地址
set serverAddress=%1

rem 接續用戶名
set user=%2

rem 接續密碼
set password=%3

rem 服務名
set serviceNm=%4

rem 服務重啟間隔
set serviceInterval=%5

rem *********************
rem 服務啟動實行
rem *********************
cd C:\WINDOWS\system32\

net use %serverAddress% /delete
net use %serverAddress%\ipc$ %password% /u:%user%

if not %ERRORLEVEL% == 0 (
  exit /b 99
)

sc %serverAddress% stop %serviceNm%
@ping -n %serviceInterval% 127.1 >nul 2>nul
sc %serverAddress% start %serviceNm%