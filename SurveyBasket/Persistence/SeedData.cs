using SurveyBasket.Abstractions;
using SurveyBasket.Abstractions.Const;

namespace SurveyBasket.Persistence;

public static class SeedData
{
    public static async Task SeedDatabaseAsync(ApplicationDbContext context)
    {
        //seed users to Member roles 
        if (!context.Users.Any(u => u.Email != "admin@SarveyBsket.com"))
        {
            var users = new List<ApplicationUser>
            {
                new()
                {
                    Id = "12345678-90AB-CDEF-1234-567890ABCDEF",
                    FirstName= "John",
                    LastName = "Doe",
                    UserName = "user1@example.com",
                    NormalizedUserName = "USER1",
                    Email = "user1@example.com",
                    NormalizedEmail = "USER1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAEJ1234567890abcdef1234567890abcdef1234567890abcdef1234567890",
                    SecurityStamp = "ABCDEF1234567890abcdef1234567890",
                    ConcurrencyStamp = "ABCDEF1234567890abcdef1234567890"
                },
                new ApplicationUser()
                {
                    Id = "23456789-0ABC-DEF1-2345-67890ABCDEF1",
                    FirstName= "Jane",
                    LastName = "Smith",
                    UserName = "user2@example.com",
                    NormalizedUserName = "USER2",
                    Email = "user2@example.com",
                    NormalizedEmail = "USER2@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAEJ1234567890abcdef1234567890abcdef1234567890abcdef1234567890",
                    SecurityStamp = "ABCDEF1234567890abcdef1234567890",
                    ConcurrencyStamp = "ABCDEF1234567890abcdef1234567890"
                },
                new ApplicationUser()
                {
                    Id = "34567890-1BCD-EF12-3456-7890ABCDEF12",
                    FirstName= "Alice",
                    LastName = "Johnson",
                    UserName = "user3@example.com",
                    NormalizedUserName = "USER3",
                    Email = "user3@example.com",
                    NormalizedEmail = "USER3@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAEJ1234567890abcdef1234567890abcdef1234567890abcdef1234567890",
                    SecurityStamp = "ABCDEF1234567890abcdef1234567890",
                    ConcurrencyStamp = "ABCDEF1234567890abcdef1234567890"
                }
            };
            context.Users.AddRange(users);
            // seed users roles
            var userRoles = new List<IdentityUserRole<string>>
            {
                new() { UserId = "12345678-90AB-CDEF-1234-567890ABCDEF", RoleId = DefaultRoles.MemberRoleId },
                new() { UserId = "23456789-0ABC-DEF1-2345-67890ABCDEF1", RoleId = DefaultRoles.MemberRoleId },
                new() { UserId = "34567890-1BCD-EF12-3456-7890ABCDEF12", RoleId = DefaultRoles.MemberRoleId }
            };
            await context.UserRoles.AddRangeAsync(userRoles);
            await context.SaveChangesAsync();
        }

        // Seed only if database already has users (assuming users are seeded separately)
        if (!context.Polls.Any())
        {
            // Get any existing user for CreatedById (using admin or first user)
            var adminUser = await context.Users.FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("No users found. Please seed users first.");

            // Seed Polls
            var polls = CreatePolls(adminUser.Id);
            context.Polls.AddRange(polls);
            await context.SaveChangesAsync();

            // Seed Questions
            var questions = CreateQuestions(adminUser.Id);
            context.Questions.AddRange(questions);
            await context.SaveChangesAsync();

            // Seed Answers
            var answers = CreateAnswers(adminUser.Id);
            context.Answers.AddRange(answers);
            await context.SaveChangesAsync();

            // Seed Votes and VoteAnswers
            // var (votes, voteAnswers) = CreateVotesAndAnswers();
            // context.Votes.AddRange(votes);
            // await context.SaveChangesAsync();
            // context.VoteAnswers.AddRange(voteAnswers);
            // await context.SaveChangesAsync();
            var votes = CreateVotesAndAnswers();
            context.Votes.AddRange(votes);
            await context.SaveChangesAsync();
        }
    }

    private static List<Poll> CreatePolls(string userId)
    {
        var polls = new List<Poll>
        {
            new()
            {
                Title = "Customer Satisfaction Survey 2026",
                Summary = "Help us improve our services by sharing your feedback about your experience with our company",
                StartAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                EndAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                IsPublished = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Title = "Product Feature Preferences",
                Summary = "Tell us which features are most important to you for future development",
                StartAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)),
                EndAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(45)),
                IsPublished = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Title = "Employee Work Environment Assessment",
                Summary = "Provide your honest feedback about working conditions and company culture",
                StartAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-60)),
                EndAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                IsPublished = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-60)
            },
            new()
            {
                Title = "Website Usability Study",
                Summary = "Share your thoughts on the usability and design of our new website",
                StartAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)),
                EndAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(40)),
                IsPublished = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-20)
            },
            new()
            {
                Title = "Annual Training Needs Assessment",
                Summary = "Identify training topics you would like to see offered in 2026",
                StartAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                EndAt = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(50)),
                IsPublished = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-10)
            }
        };

        return polls;
    }

    private static List<Question> CreateQuestions(string userId)
    {
        var questions = new List<Question>
        {
            // Poll 1: Customer Satisfaction Survey
            new()
            {
                Content = "How satisfied are you with our overall service?",
                PollId = 1,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Content = "How likely are you to recommend us to a friend?",
                PollId = 1,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Content = "What is your primary reason for using our service?",
                PollId = 1,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Content = "How responsive is our customer support team?",
                PollId = 1,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },

            // Poll 2: Product Feature Preferences
            new()
            {
                Content = "Which features would you like us to add next?",
                PollId = 2,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Content = "How important is mobile app support for you?",
                PollId = 2,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },
            new()
            {
                Content = "Would you be interested in API integration capabilities?",
                PollId = 2,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow
            },

            // Poll 3: Employee Work Environment
            new()
            {
                Content = "How satisfied are you with your work-life balance?",
                PollId = 3,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-60)
            },
            new()
            {
                Content = "Do you feel supported by your team and management?",
                PollId = 3,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-60)
            },
            new()
            {
                Content = "What is the main factor affecting your job satisfaction?",
                PollId = 3,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-60)
            },

            // Poll 4: Website Usability
            new()
            {
                Content = "How easy was it to find what you were looking for?",
                PollId = 4,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-20)
            },
            new()
            {
                Content = "How would you rate the overall design and layout?",
                PollId = 4,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-20)
            },
            new()
            {
                Content = "Is the website speed satisfactory?",
                PollId = 4,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-20)
            },

            // Poll 5: Training Needs
            new()
            {
                Content = "What technical skills would you like to develop?",
                PollId = 5,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                Content = "Which training format do you prefer?",
                PollId = 5,
                IsActive = true,
                CreatedById = userId,
                CreatedOn = DateTime.UtcNow.AddDays(-10)
            }
        };

        return questions;
    }

    private static List<Answer> CreateAnswers(string userId)
    {
        var answers = new List<Answer>
        {
            // Question 1: How satisfied are you? (Likert Scale)
            new() { Content = "Very Satisfied", QuestionId = 1, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Satisfied", QuestionId = 1, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Neutral", QuestionId = 1, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Dissatisfied", QuestionId = 1, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Very Dissatisfied", QuestionId = 1, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 2: Likelihood to recommend
            new() { Content = "Definitely Yes", QuestionId = 2, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Probably Yes", QuestionId = 2, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Not Sure", QuestionId = 2, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Probably No", QuestionId = 2, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 3: Primary reason for using service
            new() { Content = "Cost-effective", QuestionId = 3, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Quality of service", QuestionId = 3, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Reliability", QuestionId = 3, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "User-friendly interface", QuestionId = 3, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 4: Customer support responsiveness
            new() { Content = "Excellent", QuestionId = 4, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Good", QuestionId = 4, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Average", QuestionId = 4, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Poor", QuestionId = 4, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 5: Features to add
            new() { Content = "Advanced analytics dashboard", QuestionId = 5, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Multi-language support", QuestionId = 5, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Enhanced security features", QuestionId = 5, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Better integration options", QuestionId = 5, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 6: Mobile app support importance
            new() { Content = "Very Important", QuestionId = 6, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Important", QuestionId = 6, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Somewhat Important", QuestionId = 6, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Not Important", QuestionId = 6, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 7: API integration interest
            new() { Content = "Yes, definitely", QuestionId = 7, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Maybe in the future", QuestionId = 7, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Not interested", QuestionId = 7, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 8: Work-life balance
            new() { Content = "Very Satisfied", QuestionId = 8, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Satisfied", QuestionId = 8, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Neutral", QuestionId = 8, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Dissatisfied", QuestionId = 8, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 9: Team and management support
            new() { Content = "Strongly Agree", QuestionId = 9, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Agree", QuestionId = 9, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Neutral", QuestionId = 9, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Disagree", QuestionId = 9, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 10: Main job satisfaction factor
            new() { Content = "Career growth opportunities", QuestionId = 10, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Competitive compensation", QuestionId = 10, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Work environment", QuestionId = 10, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Company culture", QuestionId = 10, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 11: Finding information
            new() { Content = "Very Easy", QuestionId = 11, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Easy", QuestionId = 11, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Moderate", QuestionId = 11, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Difficult", QuestionId = 11, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 12: Design and layout rating
            new() { Content = "Excellent", QuestionId = 12, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Good", QuestionId = 12, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Fair", QuestionId = 12, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Poor", QuestionId = 12, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 13: Website speed
            new() { Content = "Yes, very fast", QuestionId = 13, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Yes, reasonably fast", QuestionId = 13, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "No, it's slow", QuestionId = 13, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 14: Technical skills to develop
            new() { Content = "Cloud Computing (AWS, Azure, GCP)", QuestionId = 14, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Machine Learning & AI", QuestionId = 14, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "DevOps & CI/CD", QuestionId = 14, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Advanced Database Design", QuestionId = 14, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },

            // Question 15: Training format preference
            new() { Content = "In-person workshops", QuestionId = 15, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Online courses", QuestionId = 15, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Hybrid (combination)", QuestionId = 15, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow },
            new() { Content = "Self-paced learning materials", QuestionId = 15, IsActive = true, CreatedById = userId, CreatedOn = DateTime.UtcNow }
        };

        return answers;
    }

    private static List<Vote> CreateVotesAndAnswers()
    {
        var votes = new List<Vote>();
        //var voteAnswers = new List<VoteAnswer>();

        // Get existing users for voting (we'll create votes for various users)
        // In a real scenario, you might have multiple users to distribute votes among

        // For now, create votes with the same user ID (as per the constraint)
        // You could modify this to create votes for different users

        // Sample vote data for Poll 1
        var pollId = 1;
        var vote = new Vote
        {
            //Id = voteId,
            UserId = "12345678-90AB-CDEF-1234-567890ABCDEF", // Using admin user
            PollId = pollId,
            SubmittedOn = DateTime.UtcNow.AddDays(-25),
            CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF",
            CreatedOn = DateTime.UtcNow.AddDays(-25)
        };
        votes.Add(vote);
        var voteId = votes[0].Id;

        // Add vote answers for Poll 1, Questions 1-4
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 1, QuestionId = 1, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Very Satisfied
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 6, QuestionId = 2, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Definitely Yes
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 11, QuestionId = 3, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Quality of service
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 15, QuestionId = 4, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Excellent

        // Sample vote data for Poll 2

        vote = new Vote
        {
            //Id = voteId,
            UserId = "23456789-0ABC-DEF1-2345-67890ABCDEF1", // Different user ID format for testing
            PollId = 2,
            SubmittedOn = DateTime.UtcNow.AddDays(-12),
            CreatedById = "23456789-0ABC-DEF1-2345-67890ABCDEF1",
            CreatedOn = DateTime.UtcNow.AddDays(-12)
        };
        votes.Add(vote);
        voteId = votes[1].Id;

        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 19, QuestionId = 5, CreatedById = "23456789-0ABC-DEF1-2345-67890ABCDEF1", CreatedOn = DateTime.UtcNow }); // Advanced analytics
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 23, QuestionId = 6, CreatedById = "23456789-0ABC-DEF1-2345-67890ABCDEF1", CreatedOn = DateTime.UtcNow }); // Very Important
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 27, QuestionId = 7, CreatedById = "23456789-0ABC-DEF1-2345-67890ABCDEF1", CreatedOn = DateTime.UtcNow }); // Yes, definitely

        // Sample vote data for Poll 3
        vote = new Vote
        {
            //Id = voteId,
            UserId = "34567890-1BCD-EF12-3456-7890ABCDEF12", // Another user ID format for testing
            PollId = 3,
            SubmittedOn = DateTime.UtcNow.AddDays(-50),
            CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12",
            CreatedOn = DateTime.UtcNow.AddDays(-50)
        };
        votes.Add(vote);
        voteId = votes[2].Id;
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 30, QuestionId = 8, CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12", CreatedOn = DateTime.UtcNow }); // Satisfied with work-life
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 34, QuestionId = 9, CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12", CreatedOn = DateTime.UtcNow }); // Agree on support
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 39, QuestionId = 10, CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12", CreatedOn = DateTime.UtcNow }); // Career growth

        // Sample vote data for Poll 4
        vote = new Vote
        {
            //Id = voteId,
            UserId = "23456789-0ABC-DEF1-2345-67890ABCDEF1",
            PollId = 4,
            SubmittedOn = DateTime.UtcNow.AddDays(-18),
            CreatedById = "23456789-0ABC-DEF1-2345-67890ABCDEF1",
            CreatedOn = DateTime.UtcNow.AddDays(-18)
        };
        votes.Add(vote);
        voteId = votes[3].Id;
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 43, QuestionId = 11, CreatedById = "23456789-0ABC-DEF1-2345-67890ABCDEF1", CreatedOn = DateTime.UtcNow }); // Very Easy
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 47, QuestionId = 12, CreatedById = "23456789-0ABC-DEF1-2345-67890ABCDEF1", CreatedOn = DateTime.UtcNow }); // Excellent design
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 51, QuestionId = 13, CreatedById = "23456789-0ABC-DEF1-2345-67890ABCDEF1", CreatedOn = DateTime.UtcNow }); // Very fast

        // Sample vote data for Poll 5
        vote = new Vote
        {
            //Id = voteId,
            UserId = "12345678-90AB-CDEF-1234-567890ABCDEF",
            PollId = 5,
            SubmittedOn = DateTime.UtcNow.AddDays(-8),
            CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF",
            CreatedOn = DateTime.UtcNow.AddDays(-8)
        };
        votes.Add(vote);
        voteId = votes[4].Id;
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 54, QuestionId = 14, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Cloud Computing
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 58, QuestionId = 15, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Online courses

        // Additional votes with different answers to have varied data
        vote = new Vote
        {
            //Id = voteId,
            UserId = "34567890-1BCD-EF12-3456-7890ABCDEF12",
            PollId = 1,
            SubmittedOn = DateTime.UtcNow.AddDays(-20),
            CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12",
            CreatedOn = DateTime.UtcNow.AddDays(-20)
        };
        votes.Add(vote);
        voteId = votes[5].Id;
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 2, QuestionId = 1, CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12", CreatedOn = DateTime.UtcNow }); // Satisfied
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 7, QuestionId = 2, CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12", CreatedOn = DateTime.UtcNow }); // Probably Yes
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 10, QuestionId = 3, CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12", CreatedOn = DateTime.UtcNow }); // Cost-effective
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 16, QuestionId = 4, CreatedById = "34567890-1BCD-EF12-3456-7890ABCDEF12", CreatedOn = DateTime.UtcNow }); // Good

        vote = new Vote
        {
            //Id = voteId,
            UserId = "12345678-90AB-CDEF-1234-567890ABCDEF",
            PollId = 3,
            SubmittedOn = DateTime.UtcNow.AddDays(-15),
            CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF",
            CreatedOn = DateTime.UtcNow.AddDays(-15)
        };
        votes.Add(vote);
        voteId = votes[6].Id;

        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 3, QuestionId = 1, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Neutral
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 8, QuestionId = 2, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Not Sure
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 12, QuestionId = 3, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Reliability
        // voteAnswers.Add(new VoteAnswer { VoteId = voteId, AnswerId = 17, QuestionId = 4, CreatedById = "12345678-90AB-CDEF-1234-567890ABCDEF", CreatedOn = DateTime.UtcNow }); // Average

        return votes;
    }
}
