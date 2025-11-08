# Script para verificar y gestionar la base de datos del foro

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Gestión de Base de Datos - Foro" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

function Show-Menu {
    Write-Host "Seleccione una opción:" -ForegroundColor Yellow
    Write-Host "1. Ver información de migraciones"
  Write-Host "2. Verificar estado de la base de datos"
    Write-Host "3. Aplicar migraciones pendientes"
  Write-Host "4. Crear nueva migración"
    Write-Host "5. Eliminar última migración"
    Write-Host "6. Recrear base de datos (DROP + UPDATE)"
    Write-Host "7. Ver tablas de la base de datos"
    Write-Host "8. Ejecutar seeding de datos"
    Write-Host "9. Salir"
    Write-Host ""
}

function Get-MigrationInfo {
    Write-Host "`n?? Información de Migraciones:" -ForegroundColor Green
    dotnet ef migrations list
}

function Test-DatabaseConnection {
    Write-Host "`n?? Verificando conexión a la base de datos..." -ForegroundColor Green
    try {
        $result = dotnet ef database update --dry-run 2>&1
        if ($LASTEXITCODE -eq 0) {
    Write-Host "? Conexión exitosa" -ForegroundColor Green
        } else {
      Write-Host "? Error de conexión" -ForegroundColor Red
        Write-Host $result
        }
    } catch {
    Write-Host "? Error: $_" -ForegroundColor Red
    }
}

function Update-Database {
    Write-Host "`n??  Aplicando migraciones pendientes..." -ForegroundColor Green
    dotnet ef database update
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Migraciones aplicadas correctamente" -ForegroundColor Green
    } else {
      Write-Host "? Error al aplicar migraciones" -ForegroundColor Red
    }
}

function New-MigrationFile {
    $name = Read-Host "`nIngrese el nombre de la migración"
    Write-Host "`n?? Creando migración '$name'..." -ForegroundColor Green
    dotnet ef migrations add $name
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Migración creada correctamente" -ForegroundColor Green
    } else {
   Write-Host "? Error al crear migración" -ForegroundColor Red
    }
}

function Remove-LastMigration {
    Write-Host "`n??  ADVERTENCIA: Esto eliminará la última migración" -ForegroundColor Yellow
    $confirm = Read-Host "¿Está seguro? (S/N)"
    if ($confirm -eq 'S' -or $confirm -eq 's') {
        Write-Host "`n???  Eliminando última migración..." -ForegroundColor Green
        dotnet ef migrations remove
     if ($LASTEXITCODE -eq 0) {
       Write-Host "? Migración eliminada correctamente" -ForegroundColor Green
      } else {
     Write-Host "? Error al eliminar migración" -ForegroundColor Red
        }
  }
}

function Reset-Database {
    Write-Host "`n??  ADVERTENCIA: Esto eliminará TODA la base de datos" -ForegroundColor Red
    Write-Host "Se perderán TODOS los datos" -ForegroundColor Red
    $confirm = Read-Host "¿Está seguro? (S/N)"
    if ($confirm -eq 'S' -or $confirm -eq 's') {
        Write-Host "`n???  Eliminando base de datos..." -ForegroundColor Yellow
     dotnet ef database drop --force
    
        Write-Host "`n?? Recreando base de datos..." -ForegroundColor Green
     dotnet ef database update
        
if ($LASTEXITCODE -eq 0) {
Write-Host "? Base de datos recreada correctamente" -ForegroundColor Green
        } else {
    Write-Host "? Error al recrear base de datos" -ForegroundColor Red
        }
    }
}

function Show-DatabaseTables {
    Write-Host "`n?? Tablas en la base de datos:" -ForegroundColor Green
  Write-Host ""
    Write-Host "Tablas de Identity (ASP.NET Core):" -ForegroundColor Cyan
    Write-Host "  - AspNetUsers"
    Write-Host "  - AspNetRoles"
    Write-Host "  - AspNetUserRoles"
    Write-Host "  - AspNetUserClaims"
    Write-Host "  - AspNetRoleClaims"
    Write-Host "  - AspNetUserLogins"
    Write-Host "  - AspNetUserTokens"
    Write-Host ""
    Write-Host "Tablas del Foro:" -ForegroundColor Cyan
  Write-Host "  - Usuarios (legacy)"
    Write-Host "  - Categorias"
    Write-Host "  - Temas"
    Write-Host "  - Mensajes"
 Write-Host ""
}

function Invoke-DataSeeding {
    Write-Host "`n?? Ejecutando seeding de datos..." -ForegroundColor Green
    Write-Host "Esto se realiza automáticamente al iniciar la aplicación" -ForegroundColor Yellow
    Write-Host "Ejecute la aplicación con: dotnet run" -ForegroundColor Yellow
}

# Menú principal
do {
  Show-Menu
    $option = Read-Host "Opción"
    
    switch ($option) {
    '1' { Get-MigrationInfo }
      '2' { Test-DatabaseConnection }
        '3' { Update-Database }
        '4' { New-MigrationFile }
        '5' { Remove-LastMigration }
        '6' { Reset-Database }
     '7' { Show-DatabaseTables }
   '8' { Invoke-DataSeeding }
        '9' { 
         Write-Host "`n?? ¡Hasta luego!" -ForegroundColor Green
      break 
        }
        default { Write-Host "`n? Opción inválida" -ForegroundColor Red }
    }
    
    if ($option -ne '9') {
        Write-Host "`nPresione cualquier tecla para continuar..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        Clear-Host
    }
} while ($option -ne '9')
