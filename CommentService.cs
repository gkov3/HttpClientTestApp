using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TestHttpClient
{

    public class Comment
    {
        public int postId { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string body { get; set; }
    }



   

    public class CommentService : BaseHttp
    {
        public CommentService()
        {
            // first test json uri i could find
            BaseUri = @"https://jsonplaceholder.typicode.com/";
        }
        public async Task<List<Comment>> GetAllComments()
        {
            return (await GetObject<Comment[]>($"comments")).ToList();
        }

        public async Task<Comment> GetComment(int id)
        {
            return await GetObject<Comment>($"comments\\{id}");
        }

        public async Task<Comment> PostComment(Comment comment)
        {
            return await  PostObject($"comments", comment);
        }
    }
}
