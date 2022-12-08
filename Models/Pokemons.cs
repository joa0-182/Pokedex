using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.Models
{
    [Table("Pokemons")]
    public class Pokemons
    {
        [Key]
        [Display(Name = "Número")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public uint Number { get; set; }

        [Display(Name = "Pokemon Base")]
        public uint? EvolvedFrom { get; set; }
        [ForeignKey("EvolvedFrom")]
        public Pokemons PokemonBase { get; set; }

        [Display(Name = "Geração")]
        [Required(ErrorMessage = "Por favor, informe a Geração")]
        public uint GenerationId { get; set; }
        [ForeignKey("GenerationId")]
        public Generation Generation { get; set; }
        
        [Display(Name = "Gênero")]
        [Required(ErrorMessage = "Por favor, informe o Gênero")]
        public uint GenderId { get; set; }
        [ForeignKey("GenderId")]
        public Gender Gender { get; set; } 

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Por favor, Informe o Nome")]
        [StringLength(30, ErrorMessage = "O Nome deve possuir no máximo 30 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [StringLength(1000, ErrorMessage = "A Descrição deve possuir no máximo 1000 caracteres")]
        public string Description { get; set; }

        [Display(Name = "Altura")]
        [Column(TypeName = "decimal(4,2)")]
        [Required(ErrorMessage = "Por favor, Informe a Altura")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode =true)]
        public double Height { get; set; }

        [Display(Name = "Peso")]
        [Column(TypeName = "decimal(6,3)")]
        [Required(ErrorMessage = "Por favor, Informe o Peso")]
        [DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode =true)]
        public double Weight { get; set; }

        [Display(Name = "Imagem")]
        [StringLength(200)]
        public string Image { get; set; }

        [Display(Name = "Link de GIF Animado")]
        [StringLength(200)]
        public string AnimatedImg { get; set; }

        public ICollection<PokemonAbilities> Abilities { get; set; } 

        public ICollection<PokemonTypes> Types { get; set; } 

        public ICollection<Weaknesses> Weaknesses { get; set; } 

    }
}

