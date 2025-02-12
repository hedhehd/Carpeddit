﻿using Carpeddit.App.Collections;
using Carpeddit.App.Helpers;
using Carpeddit.App.Models;
using Microsoft.Toolkit.Uwp;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Carpeddit.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopularPostsPage : Page
    {
        BulkConcurrentObservableCollection<PostViewModel> posts = new();

        public PopularPostsPage()
        {
            InitializeComponent();

            Loaded += Page_Loaded;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Visibility = Visibility.Collapsed;
                FooterProgress.Visibility = Visibility.Visible;

                var posts1 = await Task.Run(() => GetPosts(after: posts[posts.Count - 1].Post.Fullname));

                posts.AddRange(posts1);

                button.Visibility = Visibility.Visible;
                FooterProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Page_Loaded;

            LoadMoreButton.Visibility = Visibility.Collapsed;
            Progress.Visibility = Visibility.Visible;

            var posts1 = await Task.Run(() => GetPosts());

            posts.AddRange(posts1);

            MainList.ItemsSource = posts;

            Progress.Visibility = Visibility.Collapsed;
            LoadMoreButton.Visibility = Visibility.Visible;
        }

        private IEnumerable<PostViewModel> GetPosts(string after = "", int limit = 100, string before = "")
        {
            List<Post> frontpage = App.RedditClient.Subreddit("all").Posts.GetHot(limit: limit, after: after, before: before);
            List<PostViewModel> postViews = new();

            foreach (Post post in frontpage)
            {
                PostViewModel vm = new()
                {
                    Post = post,
                    Title = post.Title,
                    Description = post.GetDescription(),
                    Created = post.Created,
                    Subreddit = post.Subreddit,
                    Author = post.Author,
                    CommentsCount = post.Listing.NumComments
                };

                postViews.Add(vm);
            }

            return postViews;
        }
    }
}
