using System.ComponentModel.DataAnnotations;

namespace WeatherData.Models
{
    public class DayWeather
    {
        [Key]
        public int id { get; set; }
        public DateTime data { get; set; }
        public string condicao { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public int indice_uv { get; set; }
        public string condicao_desc { get; set; }
    }

}
