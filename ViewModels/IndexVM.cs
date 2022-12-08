using Pokedex.Models;
using System.Collections.Generic;

namespace Pokedex.ViewModels
{
    public class IndexVM
    {
        public List<Types> Types { get; set; }
        public List<Pokemons> Pokemons { get; set; }
    }
}