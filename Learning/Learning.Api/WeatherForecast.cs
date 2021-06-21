using System;
using System.ComponentModel.DataAnnotations;

namespace Learning.Api
{
    public class WeatherForecast
    {
        [Required(ErrorMessage = "Date不能为空")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "温度不能为空")]
        [Range(1, 100, ErrorMessage = "温度必须介于1~100之间")]
        public int TemperatureC { get; set; }

        [Required(ErrorMessage = "Date不能为空")]

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [Required(ErrorMessage = "Date不能为空")]

        public string Summary { get; set; }
    }
}
