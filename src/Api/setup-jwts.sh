dotnet user-jwts clear

dotnet user-jwts create --audience weather.dev.api --scope "write" --scope "read"
dotnet user-jwts create --audience weather.dev.api --scope "admin"