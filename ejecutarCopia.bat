@echo off

timeout /t 30 /nobreak
FOR %%X IN ("E:\Presupuesto\Release\SqlBackup.exe") DO rundll32 shell32.dll,ShellExec_RunDLL %%X