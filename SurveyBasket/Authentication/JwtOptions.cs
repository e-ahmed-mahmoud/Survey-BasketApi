using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyBasket.Authentication;

public class JwtOptions
{
    public static string SectionName = "Jwt";
    [Required]
    public string Issuer { get; set; } = string.Empty;
    [Required]
    public string Audience { get; set; } = string.Empty;
    [Required]
    public string Key { get; set; } = string.Empty;
    [Range(minimum:1 , maximum:int.MaxValue)]
    public int ExpiryInMinutes { get; set; }
}
