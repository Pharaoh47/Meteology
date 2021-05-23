using System;
namespace Meteology.Model
{
    public class Temperature : BaseEntity
    {
        public int CityId { get; set; }
        
        public DateTime Date { get; set; }

        // an EF MySQL problem with sbyte so int
        public int Min { get; set; }

        // an EF MySQL problem with sbyte so int
        public int Max { get; set; }
        
    }
}