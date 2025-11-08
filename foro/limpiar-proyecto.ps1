Write-Host "?? Limpiando proyecto del foro..." -ForegroundColor Cyan
Write-Host ""

# Guardar ubicación actual
$originalPath = Get-Location

try {
    # Limpiar compilación anterior
    Write-Host "??? Paso 1: Limpiando build anterior..." -ForegroundColor Yellow
  dotnet clean
    Write-Host "  ? Build limpiado" -ForegroundColor Green
    Write-Host ""

    # Eliminar carpetas duplicadas SalesSuite
    Write-Host "??? Paso 2: Eliminando carpetas duplicadas..." -ForegroundColor Yellow
    
    if (Test-Path "SalesSuite.Domain") {
      Remove-Item -Path "SalesSuite.Domain" -Recurse -Force
        Write-Host "  ? SalesSuite.Domain eliminada" -ForegroundColor Green
    } else {
        Write-Host "  ?? SalesSuite.Domain no existe" -ForegroundColor Gray
    }

    if (Test-Path "SalesSuite.Infrastructure") {
        Remove-Item -Path "SalesSuite.Infrastructure" -Recurse -Force
        Write-Host "  ? SalesSuite.Infrastructure eliminada" -ForegroundColor Green
    } else {
        Write-Host "  ?? SalesSuite.Infrastructure no existe" -ForegroundColor Gray
    }
    Write-Host ""

    # Eliminar controladores duplicados
    Write-Host "??? Paso 3: Eliminando controladores duplicados..." -ForegroundColor Yellow
    
    if (Test-Path "Controllers\TemasController.cs") {
        Remove-Item -Path "Controllers\TemasController.cs" -Force
Write-Host "  ? Controllers\TemasController.cs eliminado" -ForegroundColor Green
    } else {
        Write-Host "  ?? Controllers\TemasController.cs no existe" -ForegroundColor Gray
    }

  if (Test-Path "Controllers\UsuariosController.cs") {
        Remove-Item -Path "Controllers\UsuariosController.cs" -Force
        Write-Host "  ? Controllers\UsuariosController.cs eliminado" -ForegroundColor Green
    } else {
   Write-Host "  ?? Controllers\UsuariosController.cs no existe" -ForegroundColor Gray
    }
  Write-Host ""

    # Mover HomeController a Web\Controllers si es necesario
    Write-Host "?? Paso 4: Reorganizando controladores..." -ForegroundColor Yellow
    
    if (Test-Path "Controllers\HomeController.cs") {
      # Crear carpeta Web\Controllers si no existe
        if (-not (Test-Path "Web\Controllers")) {
  New-Item -Path "Web\Controllers" -ItemType Directory -Force | Out-Null
            Write-Host "  ? Carpeta Web\Controllers creada" -ForegroundColor Green
}
        
        # Mover solo si no existe en destino
        if (-not (Test-Path "Web\Controllers\HomeController.cs")) {
          Move-Item -Path "Controllers\HomeController.cs" -Destination "Web\Controllers\HomeController.cs" -Force
   Write-Host "  ? HomeController movido a Web\Controllers" -ForegroundColor Green
        } else {
        # Si ya existe en destino, eliminar el original
      Remove-Item -Path "Controllers\HomeController.cs" -Force
            Write-Host "  ? HomeController duplicado eliminado" -ForegroundColor Green
        }
    } else {
 Write-Host "  ?? HomeController ya está en Web\Controllers" -ForegroundColor Gray
    }
    Write-Host ""

    # Verificar si carpeta Controllers está vacía y eliminarla
    if (Test-Path "Controllers") {
        $files = Get-ChildItem -Path "Controllers" -File
     if ($files.Count -eq 0) {
          Remove-Item -Path "Controllers" -Recurse -Force
            Write-Host "  ? Carpeta Controllers eliminada (estaba vacía)" -ForegroundColor Green
     } else {
   Write-Host "  ?? Carpeta Controllers aún contiene $($files.Count) archivo(s)" -ForegroundColor Yellow
     foreach ($file in $files) {
  Write-Host "     - $($file.Name)" -ForegroundColor Gray
            }
        }
    }
    Write-Host ""

    # Limpiar carpetas obj y bin de forma recursiva
    Write-Host "??? Paso 5: Limpiando carpetas obj y bin..." -ForegroundColor Yellow
    $objBinFolders = Get-ChildItem -Path . -Include obj,bin -Recurse -Directory -ErrorAction SilentlyContinue
    $count = 0
    foreach ($folder in $objBinFolders) {
  Remove-Item -Path $folder.FullName -Recurse -Force -ErrorAction SilentlyContinue
        $count++
    }
    Write-Host "  ? $count carpetas obj/bin eliminadas" -ForegroundColor Green
 Write-Host ""

    # Restaurar paquetes NuGet
    Write-Host "?? Paso 6: Restaurando paquetes NuGet..." -ForegroundColor Yellow
  dotnet restore
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Paquetes restaurados" -ForegroundColor Green
    } else {
    Write-Host "  ? Error al restaurar paquetes" -ForegroundColor Red
    }
    Write-Host ""

    # Compilar proyecto
    Write-Host "?? Paso 7: Compilando proyecto..." -ForegroundColor Yellow
    dotnet build --no-incremental
    
    Write-Host ""
    Write-Host "???????????????????????????????????????????????????????" -ForegroundColor Cyan
    
    if ($LASTEXITCODE -eq 0) {
   Write-Host "? ¡PROYECTO LIMPIO Y COMPILADO EXITOSAMENTE!" -ForegroundColor Green
        Write-Host ""
        Write-Host "?? Resumen:" -ForegroundColor Cyan
        Write-Host "  ? Archivos duplicados eliminados" -ForegroundColor Green
        Write-Host "  ? Proyecto compilado sin errores" -ForegroundColor Green
        Write-Host "  ? Listo para ejecutar" -ForegroundColor Green
        Write-Host ""
        Write-Host "?? Siguiente paso: dotnet run" -ForegroundColor Yellow
    } else {
      Write-Host "? ERROR EN LA COMPILACIÓN" -ForegroundColor Red
        Write-Host ""
  Write-Host "?? Revisa los errores arriba para más detalles" -ForegroundColor Yellow
        Write-Host "?? Ver informe completo: INFORME_ERRORES_COMPILACION.md" -ForegroundColor Yellow
    }
    
  Write-Host "???????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""

} catch {
    Write-Host ""
    Write-Host "? Error durante la limpieza:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
} finally {
    # Restaurar ubicación original
    Set-Location $originalPath
}

# Verificación final
Write-Host "?? Verificación final:" -ForegroundColor Cyan
Write-Host ""

# Verificar que carpetas duplicadas no existan
$duplicatedRemoved = $true

if (Test-Path "SalesSuite.Domain") {
    Write-Host "  ?? SalesSuite.Domain aún existe" -ForegroundColor Yellow
    $duplicatedRemoved = $false
} else {
    Write-Host "  ? SalesSuite.Domain eliminada" -ForegroundColor Green
}

if (Test-Path "SalesSuite.Infrastructure") {
    Write-Host "  ?? SalesSuite.Infrastructure aún existe" -ForegroundColor Yellow
    $duplicatedRemoved = $false
} else {
    Write-Host "  ? SalesSuite.Infrastructure eliminada" -ForegroundColor Green
}

if (Test-Path "Controllers\TemasController.cs") {
    Write-Host "  ?? Controllers\TemasController.cs aún existe" -ForegroundColor Yellow
    $duplicatedRemoved = $false
} else {
    Write-Host "  ? Controllers\TemasController.cs eliminado" -ForegroundColor Green
}

Write-Host ""

if ($duplicatedRemoved -and $LASTEXITCODE -eq 0) {
    Write-Host "?? ¡Todo listo! Proyecto limpio y funcional." -ForegroundColor Green
} elseif ($LASTEXITCODE -ne 0) {
    Write-Host "?? Hay errores de compilación. Revisa el informe." -ForegroundColor Yellow
} else {
    Write-Host "?? Algunos archivos duplicados no se pudieron eliminar." -ForegroundColor Yellow
}

Write-Host ""
