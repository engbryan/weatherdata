using System.ComponentModel.DataAnnotations;

namespace WeatherData.Models
{
    public class WeatherForecast
    {
        [Key]
        public int codCidade { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public string atualizado_em { get; set; }
        public ICollection<DayWeather> clima { get; set; }
    }

}
