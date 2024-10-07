# Warning: This will clear all JWT tokens that already exists!
dotnet user-jwts clear --force

# Create JWT token with the "write" and "read" scopes
dotnet user-jwts create --audience weather.dev.api --scope "write" --scope "read"
# Create JWT token with the "admin" scope
dotnet user-jwts create --audience weather.dev.api --scope "admin"