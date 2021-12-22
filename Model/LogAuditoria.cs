using System;
using System.ComponentModel.DataAnnotations;

namespace SI_MicroServicos.Model
{
    public class LogAuditoria
    {
        [Key]
        public int ID {get;set;}
        public string DetalhesAuditoria {get;set;}
        public string User {get;set;}

    }
}