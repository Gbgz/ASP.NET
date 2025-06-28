using Microsoft.AspNetCore.Identity;

namespace AspNetCoreTodo.Models
{
    public class ApplicationUser : IdentityUser
    {
        // 扩展属性（示例）
        public string? DisplayName { get; set; }  // 用户昵称
        public DateTime? BirthDate { get; set; } // 生日
        public string? AvatarUrl { get; set; }   // 头像链接

        // 可以添加导航属性（如用户关联的待办事项）
        public List<TodoItem>? TodoItems { get; set; }
    }
}