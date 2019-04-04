@echo OFF 
SET currentdir=%~dp0
SET ExePath=DefectDataManagment\DefectDataManagment\bin\Debug\DefectDataManagment.exe
SET FullExePath=%currentdir%%ExePath% 
:: PROJECT EXE PATH, EXECUTE or NOT(TRUE/FALSE),JQL QUERY,USERNAME,PASSWORD,TASK NAME,UPDATE RECORD AND DROP TEMP TABLE
echo "Started batch Run for Defects only created Today"
echo ========================================================================================================= 
%FullExePath% "true" "project = SDC4 AND issuetype = Bug AND Created>=-12h" "vishnu.balotiya" "PA12**pa" "created" "false"
echo "Started batch Run for Defects only updated Today" 
echo ========================================================================================================= 
%FullExePath% "true" "project = SDC4 AND issuetype = Bug AND NOT created>=-10h AND updated>=-10h" "vishnu.balotiya" "PA12**pa" "updated" "true"




