using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.ValueObjects;

public record FormatoArchivo
{
    private static readonly string[] FormatosPermitidos = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".txt", ".mp3", ".mp4", ".wav", ".avi", ".mov" };
    private static readonly string[] FormatosConfiables = { ".pdf", ".docx", ".jpg", ".png" };
      public string Extension { get; private set; }
    public string NombreArchivo { get; private set; }    private FormatoArchivo(string extension, string nombreArchivo)
    {
        // Store extension without dot as tests expect it
        Extension = extension.StartsWith(".") ? extension.Substring(1) : extension;
        NombreArchivo = nombreArchivo;
    }public static FormatoArchivo Create(string nombreArchivo)
    {
        if (string.IsNullOrWhiteSpace(nombreArchivo))
            throw new DomainException("El nombre del archivo no puede estar vacío");
            
        var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
        
        if (string.IsNullOrEmpty(extension))
            throw new DomainException("El archivo debe tener una extensión válida");        if (!EsValido(nombreArchivo))
        {
            var extensionSinDot = extension.StartsWith(".") ? extension.Substring(1) : extension;
            throw new DomainException($"El formato {extensionSinDot} no está permitido");
        }
            
        return new FormatoArchivo(extension, nombreArchivo);
    }

    public static FormatoArchivo Crear(string nombreArchivo)
    {
        return Create(nombreArchivo);
    }
    
    public static bool EsValido(string nombreArchivo)
    {
        if (string.IsNullOrEmpty(nombreArchivo))
            return false;
            
        var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
        return FormatosPermitidos.Contains(extension);
    }    public bool EsConfiable()
    {
        // Extension property doesn't include the dot, so we need to add it for comparison
        var extensionConPunto = "." + Extension;
        return FormatosConfiables.Contains(extensionConPunto);
    }
}
