﻿namespace ApplicationCore.DTOs.CategoryDTOs
{
    public class GetCategoriesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public List<GetCategoriesResponse> SubCategory { get; set; }
    }
}
