fx_version 'bodacious'
game 'gta5'

author 'You'
version '1.0.0'

fxdk_watch_command 'dotnet' {'watch', '--project', 'Client/Offense_Defense.Client.csproj', 'publish', '--configuration', 'Release'}
fxdk_watch_command 'dotnet' {'watch', '--project', 'Server/Offense_Defense.Server.csproj', 'publish', '--configuration', 'Release'}

ui_page 'Client/html/ui.html'

files {
    'Client/bin/Release/**/publish/*.dll',
    'Newtonsoft.Json.dll',
    'Client/html/ui.html',
    'Client/html/ui.css',
    'Client/html/ui.js'
} 
    

client_script 'Client/bin/Release/**/publish/*.net.dll'
server_script 'Server/bin/Release/**/publish/*.net.dll'
