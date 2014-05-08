@set SCRIPT_DIR=%~dp0

"C:\Program Files (x86)\IIS Express\iisexpress.exe" /port:5512 -path:"%SCRIPT_DIR:~0,-1%" 

start http://localhost:5512/
