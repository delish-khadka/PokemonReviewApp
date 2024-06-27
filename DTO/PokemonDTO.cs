﻿namespace PokemonReviewApp.DTO
{
    public class PokemonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Image { get; set; }
        public List<string>? Categories { get; set; }
    }
}
