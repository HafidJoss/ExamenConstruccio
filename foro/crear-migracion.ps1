# ========================================
# Script para Crear Migración Inicial
# ========================================

Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "  Crear Migración InitialCreate" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""

# Navegar a la carpeta del proyecto
$projectPath = "foro"
if (Test-Path $projectPath) {
    Set-Location $projectPath
Write-Host "? Navegado a: $projectPath" -ForegroundColor Green
} else {
    Write-Host "? Error: No se encontró la carpeta $projectPath" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Verificar si ya existe una migración
if (Test-Path "Migrations") {
    Write-Host "? Ya existe una carpeta de Migrations" -ForegroundColor Yellow
    $response = Read-Host "¿Deseas eliminar las migraciones existentes? (S/N)"
    
    if ($response -eq "S" -or $response -eq "s") {
     Write-Host "Eliminando migraciones..." -ForegroundColor Yellow
     Remove-Item -Path "Migrations" -Recurse -Force
        Write-Host "? Migraciones eliminadas" -ForegroundColor Green
    }
}

Write-Host ""

# Crear migración
Write-Host "Creando migración InitialCreate..." -ForegroundColor Cyan
try {
    dotnet ef migrations add InitialCreate
    Write-Host ""
    Write-Host "? Migración InitialCreate creada exitosamente" -ForegroundColor Green
} catch {
    Write-Host "? Error al crear la migración" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host ""

# Preguntar si desea aplicar la migración
$applyMigration = Read-Host "¿Deseas aplicar la migración ahora? (S/N)"

if ($applyMigration -eq "S" -or $applyMigration -eq "s") {
  Write-Host ""
    Write-Host "Aplicando migración..." -ForegroundColor Cyan
    
    try {
        dotnet ef database update
        Write-Host ""
Write-Host "? Migración aplicada exitosamente" -ForegroundColor Green
    } catch {
      Write-Host "? Error al aplicar la migración" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    Write-Host "? Base de datos creada y migración aplicada" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "? Migración creada pero no aplicada" -ForegroundColor Yellow
    Write-Host "? La migración se aplicará automáticamente al iniciar la aplicación" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "  Resumen" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "? Migración InitialCreate creada" -ForegroundColor Green

if ($applyMigration -eq "S" -or $applyMigration -eq "s") {
    Write-Host "? Migración aplicada a la base de datos" -ForegroundColor Green
    Write-Host "? DataSeeder se ejecutará automáticamente al iniciar" -ForegroundColor Green
} else {
    Write-Host "? Ejecuta 'dotnet run' para aplicar la migración automáticamente" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Comandos útiles:" -ForegroundColor Cyan
Write-Host "  - Ver migraciones:    dotnet ef migrations list" -ForegroundColor Gray
Write-Host "- Aplicar migración:  dotnet ef database update" -ForegroundColor Gray
Write-Host "  - Eliminar BD:        dotnet ef database drop" -ForegroundColor Gray
Write-Host "  - Ejecutar app:       dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "===========================================" -ForegroundColor Cyan

# Volver a la carpeta raíz
Set-Location ..
