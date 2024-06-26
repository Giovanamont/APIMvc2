using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIMvc.Models
{
    public class FMViewModel
    {
        public int Id { get; set; }
        public string Frase { get; set; } = "";
        public string Autor { get; set; } = "";
        public string Sentimento { get; set; } = "";
    }
}