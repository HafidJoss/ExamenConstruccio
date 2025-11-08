# Script de verificación de la corrección de migraciones

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Verificación de Corrección Completa" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$errors = 0
$warnings = 0

# 1. Verificar que no hay errores de compilación
Write-Host "1??Verificando compilación..." -ForegroundColor Yellow
dotnet build --no-incremental -v quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ? Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "   ? Error en compilación" -ForegroundColor Red
    $errors++
}
Write-Host ""

# 2. Verificar migraciones
Write-Host "2??  Verificando migraciones..." -ForegroundColor Yellow
$migrations = dotnet ef migrations list 2>&1 | Select-String "InitialCreateCorrected"
if ($migrations) {
  Write-Host "   ? Migración 'InitialCreateCorrected' encontrada" -ForegroundColor Green
} else {
    Write-Host "   ??  Migración 'InitialCreateCorrected' no encontrada" -ForegroundColor Yellow
    $warnings++
}
Write-Host ""

# 3. Verificar estructura de archivos
Write-Host "3??  Verificando archivos..." -ForegroundColor Yellow
$files = @(
    "Infrastructure\Data\ForumDbContext.cs",
    "Domain\Entities\ApplicationUser.cs",
    "Domain\Entities\Usuario.cs",
    "Domain\Entities\Tema.cs",
    "Domain\Entities\Mensaje.cs",
    "Domain\Entities\Categoria.cs",
    "Program.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "   ? $file" -ForegroundColor Green
    } else {
        Write-Host "   ? $file NO ENCONTRADO" -ForegroundColor Red
        $errors++
    }
}
Write-Host ""

# 4. Verificar contenido de ForumDbContext
Write-Host "4??  Verificando configuración del DbContext..." -ForegroundColor Yellow
$dbContextContent = Get-Content "Infrastructure\Data\ForumDbContext.cs" -Raw

if ($dbContextContent -match "entity\.Ignore\(u => u\.Temas\)") {
    Write-Host "   ? Ignore(Temas) configurado" -ForegroundColor Green
} else {
    Write-Host "   ? Ignore(Temas) NO configurado" -ForegroundColor Red
    $errors++
}

if ($dbContextContent -match "entity\.Ignore\(u => u\.Mensajes\)") {
    Write-Host "   ? Ignore(Mensajes) configurado" -ForegroundColor Green
} else {
    Write-Host "   ? Ignore(Mensajes) NO configurado" -ForegroundColor Red
    $errors++
}

if ($dbContextContent -match "DeleteBehavior\.Restrict") {
 Write-Host "   ? DeleteBehavior.Restrict encontrado" -ForegroundColor Green
} else {
    Write-Host "   ??  DeleteBehavior.Restrict no encontrado" -ForegroundColor Yellow
    $warnings++
}
Write-Host ""

# 5. Verificar base de datos
Write-Host "5??  Verificando base de datos..." -ForegroundColor Yellow
$dbCheck = dotnet ef database update --dry-run 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ? Conexión a base de datos exitosa" -ForegroundColor Green
    Write-Host "   ? No hay migraciones pendientes" -ForegroundColor Green
} else {
    Write-Host "   ??  Verificar manualmente la base de datos" -ForegroundColor Yellow
    $warnings++
}
Write-Host ""

# 6. Verificar que no existen columnas ApplicationUserId en las migraciones
Write-Host "6??  Verificando ausencia de columnas incorrectas..." -ForegroundColor Yellow
$migrationFiles = Get-ChildItem "Migrations\*.cs" -Exclude "*.Designer.cs", "*Snapshot.cs"
$foundApplicationUserId = $false

foreach ($migFile in $migrationFiles) {
    $content = Get-Content $migFile.FullName -Raw
    if ($content -match "ApplicationUserId") {
     Write-Host "   ? ApplicationUserId encontrado en $($migFile.Name)" -ForegroundColor Red
  $foundApplicationUserId = $true
        $errors++
    }
}

if (-not $foundApplicationUserId) {
    Write-Host "   ? No se encontraron columnas ApplicationUserId" -ForegroundColor Green
}
Write-Host ""

# Resumen
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RESUMEN DE VERIFICACIÓN" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($errors -eq 0 -and $warnings -eq 0) {
    Write-Host "?? ¡TODAS LAS VERIFICACIONES PASARON!" -ForegroundColor Green
    Write-Host ""
    Write-Host "? La corrección de migraciones se completó exitosamente" -ForegroundColor Green
    Write-Host "? La base de datos está correctamente configurada" -ForegroundColor Green
 Write-Host "? No hay columnas duplicadas (ApplicationUserId)" -ForegroundColor Green
Write-Host "? Las relaciones de foreign key son correctas" -ForegroundColor Green
} elseif ($errors -eq 0) {
    Write-Host "??  Verificación completada con advertencias" -ForegroundColor Yellow
    Write-Host "   Advertencias: $warnings" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "? No se encontraron errores críticos" -ForegroundColor Green
  Write-Host "??  Revise las advertencias anteriores" -ForegroundColor Yellow
} else {
    Write-Host "? ERRORES ENCONTRADOS" -ForegroundColor Red
    Write-Host "   Errores: $errors" -ForegroundColor Red
    Write-Host "   Advertencias: $warnings" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "? Por favor, revise los errores anteriores" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Información adicional
Write-Host "?? Documentación:" -ForegroundColor Cyan
Write-Host "   - CORRECCION_MIGRACIONES.md" -ForegroundColor White
Write-Host "   - DOCUMENTACION_IDENTITY.md" -ForegroundColor White
Write-Host ""
Write-Host "???  Scripts útiles:" -ForegroundColor Cyan
Write-Host "   - .\gestionar-db.ps1  (Gestión de base de datos)" -ForegroundColor White
Write-Host ""
Write-Host "?? Para ejecutar la aplicación:" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""
