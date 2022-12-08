using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.Models
{
    [Table("Abilities")]
    public class Abilities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Por favor, Informe o Nome")]
        [StringLength(30, ErrorMessage = "O Nome da Habilidade deve possuir no máximo 30 caracteres")]
        public string Name { get; set; } 

        public ICollection<PokemonAbilities> PokemonsWithAbility { get; set; } 
    }
}

