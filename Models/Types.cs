using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.Models
{
    [Table("Types")]
    public class Types
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Por favor, Informe o Nome")]
        [StringLength(30, ErrorMessage = "O Nome do Tipo deve possuir no máximo 30 caracteres")]
        public string Name { get; set; } 

        [Display(Name = "Cor de Exibição")]
        [Required(ErrorMessage = "Informe a Cor")]
        [StringLength(7, ErrorMessage = "A Cor deve possuir no máximo 7 caracteres")]
        public string Color { get; set; } 

        public ICollection<PokemonTypes> PokemonsOfThisType { get; set; } 
        public ICollection<Weaknesses> PokemonsWithThisWeakness { get; set; } 
    }
}

