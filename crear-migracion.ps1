# Script PowerShell para crear la migración InitialCreate
# Ejecutar desde la raíz del proyecto

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Creando Migración InitialCreate" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en el directorio correcto
if (-Not (Test-Path "SalesSuite.sln")) {
    Write-Host "Error: No se encuentra SalesSuite.sln" -ForegroundColor Red
    Write-Host "Asegúrate de ejecutar este script desde la raíz del proyecto" -ForegroundColor Yellow
    exit 1
}

Write-Host "1. Limpiando solución..." -ForegroundColor Yellow
dotnet clean

Write-Host ""
Write-Host "2. Compilando solución..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Error: La compilación falló" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "3. Creando migración InitialCreate..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate `
    --project SalesSuite.Infrastructure `
    --startup-project SalesSuite.Web `
    --context ForumDbContext `
    --output-dir Data/Migrations

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Error: Falló la creación de la migración" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Migración creada exitosamente!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Próximos pasos:" -ForegroundColor Cyan
Write-Host "1. Revisa la migración en: SalesSuite.Infrastructure/Data/Migrations/" -ForegroundColor White
Write-Host "2. Ejecuta la aplicación para aplicar la migración automáticamente" -ForegroundColor White
Write-Host "   O usa: dotnet ef database update --project SalesSuite.Infrastructure --startup-project SalesSuite.Web" -ForegroundColor White
Write-Host ""
