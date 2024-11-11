using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("Posts")]
    public IEnumerable<Post> GetPosts()
    {
        string getPostsSql = @"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] 
                FROM TutorialAppSchema.Posts";

        return _dapper.LoadData<Post>(getPostsSql);
    }

    [HttpGet("PostsByUser/{userId}")]
    public IEnumerable<Post> GetPostsByUser(int userId)
    {
        string getPostsByUserSql = $@"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                WHERE UserId = {userId}";

        return _dapper.LoadData<Post>(getPostsByUserSql);
    }

    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string? myUserId = this.User.FindFirst("userId")?.Value;
        string getPostsSql = $@"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                WHERE UserId = {myUserId}";

        return _dapper.LoadData<Post>(getPostsSql);
    }

    [HttpGet("PostSingle/{postId}")]
    public Post GetSinglePost(int postId)
    {
        string getPostFromIdSql = $@"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                WHERE PostId = {postId}";

        return _dapper.LoadDataSingle<Post>(getPostFromIdSql);
    }

    [HttpGet("PostsBySearch/{searchParam}")]
    public IEnumerable<Post> PostsBySearch(string searchParam)
    {
        string getPostsSql = $@"SELECT [PostId],
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                WHERE PostTitle LIKE '%{searchParam}%'
                    OR  PostContent LIKE '%{searchParam}%'";

        return _dapper.LoadData<Post>(getPostsSql);
    }


/* TODO:
"escape" the apostrophe for the SQL query by replacing it with two apostrophes, i.e.,
 stringValue.Replace("'", "''")
OR, use an instance of either the SqlParameter or DynamicParameter class, 
Dominic recommends the latter approach
*/

    [HttpPost("Post")]
    public IActionResult AddPost(PostToAddDto postToAdd)
    {
        // Get userId from token
        string? userId = this.User.FindFirst("userId")?.Value;
        string addPostSql = $@"INSERT INTO TutorialAppSchema.Posts (
                            [UserId],
                            [PostTitle],
                            [PostContent],
                            [PostCreated],
                            [PostUpdated]
                            )
                            VALUES ({userId},
                                   '{postToAdd.PostTitle}',
                                   '{postToAdd.PostContent}',
                                    GetDate(), GetDate())";
        // If we change one row, assume we are good
        if (_dapper.ExecuteSql(addPostSql))
        {
            return Ok();
        }
        throw new Exception("Failed to Create Post");
    }

    [HttpPut("Post")]
    public IActionResult EditPost(PostToEditDto postToEdit)
    {
        // Get userId from token so that only user who
        // created the post can edit it
        string? userId = this.User.FindFirst("userId")?.Value;

        string editPostSql = $@"UPDATE TutorialAppSchema.Posts 
                SET PostTitle = '{postToEdit.PostTitle}',
                    PostContent = '{postToEdit.PostContent}', 
                    PostUpdated = GETDATE()
                WHERE PostId = {postToEdit.PostId}
                  AND UserId = {userId}";

        if (_dapper.ExecuteSql(editPostSql))
        {
            return Ok();
        }
        throw new Exception($"Failed to edit post {postToEdit.PostId}!");
    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        // Get userId from token so that only user who
        // created the post can delete it
        string? userId = this.User.FindFirst("userId")?.Value;

        string deletePostSql = $@"DELETE FROM TutorialAppSchema.Posts
                                 WHERE PostId = {postId}
                                    AND UserId = {userId}";

        if (_dapper.ExecuteSql(deletePostSql))
        {
            return Ok();
        }
        throw new Exception($"Failed to delete post {postId}!");
   }
}