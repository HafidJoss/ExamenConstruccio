# Script PowerShell para aplicar migraciones a la base de datos
# Ejecutar desde la raíz del proyecto

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Aplicando Migraciones a la Base de Datos" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en el directorio correcto
if (-Not (Test-Path "SalesSuite.sln")) {
    Write-Host "Error: No se encuentra SalesSuite.sln" -ForegroundColor Red
    Write-Host "Asegúrate de ejecutar este script desde la raíz del proyecto" -ForegroundColor Yellow
    exit 1
}

Write-Host "Aplicando migraciones..." -ForegroundColor Yellow
dotnet ef database update `
    --project SalesSuite.Infrastructure `
    --startup-project SalesSuite.Web `
    --context ForumDbContext

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Error: Falló la aplicación de migraciones" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Migraciones aplicadas exitosamente!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "La base de datos está lista para usar." -ForegroundColor Cyan
Write-Host "El seeding de datos se ejecutará automáticamente al iniciar la aplicación." -ForegroundColor Cyan
Write-Host ""
