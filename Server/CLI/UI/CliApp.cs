using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private IUserRepository userRepository;
    private ICommentRepository commentRepository;
    private IPostRepository postRepository;

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    public async Task StartAsync()
    {
        bool isRunning = true;

        while (isRunning)
        {
            try
            {
                Console.Clear();
            } catch {}
            Console.WriteLine("=== Main Menu ===");
            Console.WriteLine("1. View all posts");
            Console.WriteLine("2. Create new post");
            Console.WriteLine("3. Manage/Edit post");
            Console.WriteLine("4. View single post");
            Console.WriteLine("5. View all users");
            Console.WriteLine("6. Create new user");
            Console.WriteLine("7. Manage/Edit user");
            Console.WriteLine("8. Create new comment");
            Console.WriteLine("9. View all comments");
            Console.WriteLine("10. Edit a comment");
            Console.WriteLine("11. Exit");
            Console.Write("\nChoose an option (1-11): ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                await ViewAllPosts();
            }
            else if (choice == "2")
            {
                await CreatePost();
            }
            else if (choice == "3")
            {
                await EditPost();
            }
            else if (choice == "4")
            {
                await ViewSinglePost();
            }
            else if (choice == "5")
            {
                await ViewAllUsers();
            }
            else if (choice == "6")
            {
                await CreateUser();
            }
            else if (choice == "7")
            {
                await EditUser();
            }
            else if (choice == "8")
            {
                await CreateComment();
            }
            else if (choice == "9")
            {
                await ViewAllComments();
            }
            else if (choice == "10")
            {
                await EditComment();
            }
            else if (choice == "11")
            {
                isRunning = false;
                Console.WriteLine("Exiting application...");
            }
            else
            {
                Console.WriteLine("Invalid choice. Press key to continue.");
                try
                {
                    Console.ReadKey();
                }
                catch
                {
                    Console.ReadLine();
                }
            }
        }
    }

    private async Task ViewAllPosts()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== All Posts ===");

        List<Post> posts = postRepository.GetMany().ToList();

        if (posts.Count == 0)
        {
            Console.WriteLine("No posts available.");
        }
        else
        {
            foreach (Post post in posts)
            {
                Console.WriteLine("ID: " + post.Id + " | Title: " + post.Title + " | Author ID: " + post.UserId);
            }
        }

        Console.WriteLine("\nPress any key to return...");
        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task CreatePost()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== Create Post ===");

        Console.Write("Enter User ID: ");
        string userIdInput = Console.ReadLine();
        int userId = Convert.ToInt32(userIdInput);

        Console.Write("Enter Title: ");
        string title = Console.ReadLine();

        Console.Write("Enter Body: ");
        string body = Console.ReadLine();

        Post newPost = new Post(title, body, userId);
        await postRepository.AddAsync(newPost);

        Console.WriteLine("\nPost created! Press any key to return...");
        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task EditPost()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== Edit Post ===");

        Console.Write("Enter Post ID to edit: ");
        string idInput = Console.ReadLine();
        int id = Convert.ToInt32(idInput);

        Post post = await postRepository.GetSingleAsync(id);

        if (post != null)
        {
            Console.Write("Enter New Title (Current: " + post.Title + "): ");
            string newTitle = Console.ReadLine();
            if (newTitle != "")
            {
                post.Title = newTitle;
            }

            Console.Write("Enter New Body: ");
            string newBody = Console.ReadLine();
            if (newBody != "")
            {
                post.Body = newBody;
            }

            await postRepository.UpdateAsync(post);
            Console.WriteLine("\nPost updated successfully! Press any key to return...");
        }
        else
        {
            Console.WriteLine("Post not found.");
        }
        
        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task ViewSinglePost()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== View Single Post ===");

        Console.Write("Enter Post ID: ");
        string idInput = Console.ReadLine();
        int id = Convert.ToInt32(idInput);

        Post post = await postRepository.GetSingleAsync(id);

        if (post != null)
        {
            Console.WriteLine("ID: " + post.Id);
            Console.WriteLine("Title: " + post.Title);
            Console.WriteLine("Body: " + post.Body);
            Console.WriteLine("Author ID: " + post.UserId);
        }
        else
        {
            Console.WriteLine("Post not found.");
        }
        
        Console.WriteLine("\nPress any key to return...");
        
        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task ViewAllUsers()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== All Users ===");

        List<User> users = userRepository.GetMany().ToList();

        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
        }
        else
        {
            foreach (User user in users)
            {
                Console.WriteLine("ID: " + user.Id + " | Username: " + user.Username);
            }
        }

        Console.WriteLine("\nPress any key to return...");
        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task CreateUser()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== Create User ===");

        Console.Write("Enter Username: ");
        string username = Console.ReadLine();

        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        User newUser = new User(username, password);
        await userRepository.AddAsync(newUser);

        Console.WriteLine("\nUser created! Press any key to return...");
        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task EditUser()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== Edit User ===");

        Console.Write("Enter User ID to edit: ");
        string idInput = Console.ReadLine();
        int id = Convert.ToInt32(idInput);

        User user = await userRepository.GetSingleAsync(id);

        if (user != null)
        {
            Console.Write("Enter New Username (Current: " + user.Username + "): ");
            string newUsername = Console.ReadLine();
            if (newUsername != "")
            {
                user.Username = newUsername;
            }

            Console.Write("Enter New Password: ");
            string newPassword = Console.ReadLine();
            if (newPassword != "")
            {
                user.Password = newPassword;
            }

            await userRepository.UpdateAsync(user);
            Console.WriteLine("\nUser updated successfully! Press any key to return...");
        }
        else
        {
            Console.WriteLine("User not found.");
        }

        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task ViewAllComments()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== All Comments ===");

        List<Comment> comments = commentRepository.GetMany().ToList();

        if (comments.Count == 0)
        {
            Console.WriteLine("No comments found.");
        }
        else
        {
            foreach (Comment comment in comments)
            {
                Console.WriteLine("ID: " + comment.Id + " | Post ID: " + comment.PostId + " | User ID: " + comment.UserId);
                Console.WriteLine("Body: " + comment.Body);
                Console.WriteLine("----------------------------------");
            }
        }

        Console.WriteLine("\nPress any key to return...");
        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task CreateComment()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== Create Comment ===");

        Console.Write("Enter Post ID: ");
        string postIdInput = Console.ReadLine();
        int postId = Convert.ToInt32(postIdInput);

        Console.Write("Enter User ID: ");
        string userIdInput = Console.ReadLine();
        int userId = Convert.ToInt32(userIdInput);

        Console.Write("Enter Comment Text: ");
        string body = Console.ReadLine();

        Comment newComment = new Comment(body, postId, userId);
        await commentRepository.AddAsync(newComment);

        Console.WriteLine("\nComment created! Press any key to return...");
        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }

    private async Task EditComment()
    {
        try
        {
            Console.Clear();
        } catch {}
        Console.WriteLine("=== Edit Comment ===");

        Console.Write("Enter Comment ID to edit: ");
        string idInput = Console.ReadLine();
        int id = Convert.ToInt32(idInput);

        Comment comment = await commentRepository.GetSingleAsync(id);

        if (comment != null)
        {
            Console.Write("Enter New Body (Current: " + comment.Body + "): ");
            string newBody = Console.ReadLine();
            if (newBody != "")
            {
                comment.Body = newBody;
            }

            await commentRepository.UpdateAsync(comment);
            Console.WriteLine("\nComment updated successfully! Press any key to return...");
        }
        else
        {
            Console.WriteLine("Comment not found.");
        }

        try
        {
            Console.ReadKey();
        }
        catch
        {
            Console.ReadLine();
        }
    }
    
}