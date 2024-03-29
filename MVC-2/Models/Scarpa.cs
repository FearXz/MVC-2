﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC_2.Models
{
    public class Scarpa
    {
        public int IdScarpa { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        [DataType(DataType.Currency)]
        public decimal Prezzo { get; set; }
        public string ImmagineCopertina { get; set; }
        public string ImmagineNome { get; set; }
        public bool Attivo { get; set; }

        public List<ImmagineXtra> ListaImmagini = new List<ImmagineXtra>();

    }
}