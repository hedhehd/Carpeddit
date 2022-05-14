﻿using Carpeddit.App.Collections;
using Carpeddit.App.Models;
using Reddit.Controllers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Carpeddit.App.Pages
{
    public sealed partial class YourProfilePage : Page
    {
        private BulkConcurrentObservableCollection<PostViewModel> posts;
        private User user = App.RedditClient.Account.Me;

        public YourProfilePage()
        {
            InitializeComponent();

            posts = new();
            Loaded += Page_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Account account)
            {
                user = account.Me;
            } else if (e.Parameter is User _user)
            {
                user = _user;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                var posts1 = await Task.Run(async () =>
                {
                    return await GetPostsAsync(after: posts[posts.Count - 1].Post.Fullname);
                });

                posts.AddRange(posts1);

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Page_Loaded;

            if (user != App.RedditClient.Account.Me)
            {
                FloatingSplit.Visibility = Visibility.Collapsed;
            } else
            {
                FloatingSplit.Visibility = Visibility.Visible;
            }

            LoadMoreButton.Visibility = Visibility.Collapsed;
            ProgressR.Visibility = Visibility.Visible;

            var posts1 = await Task.Run(async () =>
            {
                return await GetPostsAsync();
            });

            posts.AddRange(posts1);

            MainList.ItemsSource = posts;

            ProgressR.Visibility = Visibility.Collapsed;
            LoadMoreButton.Visibility = Visibility.Visible;
        }

        private async Task<ObservableCollection<PostViewModel>> GetPostsAsync(string after = "", int limit = 13, string before = "")
        {
            List<Post> frontpage = user.GetPostHistory(limit: 13, after: after, before: before);
            ObservableCollection<PostViewModel> postViews = new();

            foreach (Post post in frontpage)
            {
                PostViewModel vm = new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = GetPostDesc(post),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                };

                postViews.Add(vm);
            }

            return postViews;
        }

        private string GetPostDesc(Post post)
        {
            if (post is LinkPost linkPost)
            {
                return linkPost.URL;
            }
            else if (post is SelfPost selfPost)
            {
                return selfPost.SelfText;
            }

            return "No content";
        }
    }
}
