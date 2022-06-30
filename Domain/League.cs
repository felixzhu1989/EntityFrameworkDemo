using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// 联赛
    /// </summary>
    public class League
    {
        public int Id { get; set; }
        [MaxLength(100)]//限制字符串长度为100
        [Required]//必填项目
        public string Name { get; set; }
        [Required,MaxLength(50)]
        public string Country { get; set; }
    }
}