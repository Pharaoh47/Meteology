using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

namespace Meteology.Model
{
    public class City : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public List<Temperature> Temperatures { get; set; }

        public void UpdateMeasurements(DateTime date, int min, int max)
        {
            Temperature temp = Temperatures.Where(t => t.Date == date).FirstOrDefault();
            if (temp == null)
            {
                temp = new Temperature() { Date = date };
                Temperatures.Add(temp);
            }
            temp.Min = min;
            temp.Max = max;
        }
    }
}