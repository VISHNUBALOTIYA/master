@echo OFF 
SET currentdir=%~dp0
SET ExePath=DefectDataManagment\DefectDataManagment\bin\Debug\DefectDataManagment.exe
SET FullExePath=%currentdir%%ExePath% 
:: PROJECT EXE PATH, EXECUTE or NOT(TRUE/FALSE),JQL QUERY,USERNAME,PASSWORD,TASK NAME,UPDATE RECORD AND DROP TEMP TABLE
echo "Started batch Run for OneTime Run"
echo ========================================================================================================= 
%FullExePath% "false" "project = SDC4 AND issuetype = Bug" "vishnu.balotiya" "PA12**pa" "onetimerun" "None"





