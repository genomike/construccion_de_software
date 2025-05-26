using EtapaDeJuicio.Domain.Exceptions;
using EtapaDeJuicio.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EtapaDeJuicio.Domain.Entities.Audiencias
{
    public class Contrainterrogatorio
    {
        public Guid Id { get; private set; }
        public Guid InterrogatorioOriginalId { get; private set; } // ID del interrogatorio al que responde
        public string Descripcion { get; private set; }
        public DateTime FechaHora { get; private set; }
        private readonly List<Pregunta> _preguntas = new List<Pregunta>();
        public IReadOnlyList<Pregunta> Preguntas => _preguntas.AsReadOnly();
        public bool EstaCompleto { get; private set; }

        private Contrainterrogatorio(Guid id, Guid interrogatorioOriginalId, string descripcion, DateTime fechaHora)
        {
            if (id == Guid.Empty)
                throw new DomainException("El ID del contrainterrogatorio no puede estar vacío.");

            if (interrogatorioOriginalId == Guid.Empty)
                throw new DomainException("El ID del interrogatorio original no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(descripcion))
                throw new DomainException("La descripción del contrainterrogatorio es obligatoria.");

            Id = id;
            InterrogatorioOriginalId = interrogatorioOriginalId;
            Descripcion = descripcion;
            FechaHora = fechaHora;
            EstaCompleto = false;
        }

        public static Contrainterrogatorio Crear(Guid interrogatorioOriginalId, string descripcion, DateTime fechaHora)
        {
            return new Contrainterrogatorio(Guid.NewGuid(), interrogatorioOriginalId, descripcion, fechaHora);
        }

        public static Contrainterrogatorio Crear(Guid id, Guid interrogatorioOriginalId, string descripcion, DateTime fechaHora)
        {
            return new Contrainterrogatorio(id, interrogatorioOriginalId, descripcion, fechaHora);
        }        public void AgregarPregunta(string textoPregunta)
        {
            if (EstaCompleto)
                throw new DomainException("No se pueden agregar preguntas a un contrainterrogatorio completo.");

            if (string.IsNullOrWhiteSpace(textoPregunta))
                throw new DomainException("El texto de la pregunta no puede estar vacío.");

            if (!ValidadorPreguntas.EsValida(textoPregunta, TipoInterrogatorio.Contrainterrogatorio))
                throw new DomainException("La pregunta no es válida para contrainterrogatorio.");

            var pregunta = Pregunta.Crear(textoPregunta, TipoPregunta.Abierta);
            _preguntas.Add(pregunta);
        }public void RegistrarRespuesta(Guid preguntaId, string respuesta)
        {
            var pregunta = _preguntas.FirstOrDefault(p => p.Id == preguntaId);
            if (pregunta == null)
                throw new DomainException("La pregunta no existe en este contrainterrogatorio.");

            pregunta.Responder(respuesta);
        }

        public void MarcarComoCompleto()
        {
            if (!_preguntas.Any())
                throw new DomainException("El contrainterrogatorio debe tener al menos una pregunta para ser completado.");
            
            EstaCompleto = true;
        }
    }
}
