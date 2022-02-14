﻿using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class YourProfilePage : Page
    {
        private readonly List<Post> posts = App.RedditClient.Account.Me.PostHistory;

        public YourProfilePage()
        {
            InitializeComponent();
        }

        private void UpvoteButton_Click(object sender, RoutedEventArgs e)
        {
            Post post = (sender as ToggleButton).Tag as Post;
            post.UpvoteAsync();
        }

        private void DownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            Post post = (sender as ToggleButton).Tag as Post;
            post.DownvoteAsync();
        }
    }
}
